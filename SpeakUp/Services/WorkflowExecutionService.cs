using SpeakUp.Executor;
using SpeakUp.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SpeakUp.Services;

/// <summary>
/// Service for executing workflows
/// </summary>
public interface IWorkflowExecutionService
{
    /// <summary>
    /// Execute a workflow
    /// </summary>
    Task<WorkflowExecutionResult> ExecuteWorkflowAsync(int workflowId, Dictionary<string, object?>? initialVariables = null);

    /// <summary>
    /// Execute a workflow by name
    /// </summary>
    Task<WorkflowExecutionResult> ExecuteWorkflowByNameAsync(string workflowName, Dictionary<string, object?>? initialVariables = null);

    /// <summary>
    /// Test a workflow step
    /// </summary>
    Task<WorkflowExecutionResult> TestStepAsync(WorkflowStep step, Dictionary<string, object?> variables);

    /// <summary>
    /// Validate a workflow
    /// </summary>
    Task<(bool isValid, List<string> errors)> ValidateWorkflowAsync(int workflowId);

    /// <summary>
    /// Cancel a running workflow
    /// </summary>
    void CancelWorkflow(int workflowId);
}

/// <summary>
/// Implementation of workflow execution service
/// </summary>
internal sealed class WorkflowExecutionService : IWorkflowExecutionService
{
    private readonly IWorkflowService _workflowService;
    private readonly IExecutor _executor;
    private readonly IErrorHandlingService _errorHandler;
    private readonly Dictionary<int, CancellationTokenSource> _runningWorkflows = new();

    public WorkflowExecutionService(
        IWorkflowService workflowService,
        IExecutor executor,
        IErrorHandlingService errorHandler)
    {
        _workflowService = workflowService;
        _executor = executor;
        _errorHandler = errorHandler;
    }

    public async Task<WorkflowExecutionResult> ExecuteWorkflowAsync(
        int workflowId,
        Dictionary<string, object?>? initialVariables = null)
    {
        var workflow = await _workflowService.GetWorkflowAsync(workflowId);
        if (workflow == null)
        {
            return new WorkflowExecutionResult
            {
                Success = false,
                ErrorMessage = $"Workflow {workflowId} not found"
            };
        }

        if (!workflow.IsEnabled)
        {
            return new WorkflowExecutionResult
            {
                Success = false,
                ErrorMessage = $"Workflow '{workflow.Name}' is disabled"
            };
        }

        var steps = await _workflowService.GetWorkflowStepsAsync(workflowId);
        if (steps.Count == 0)
        {
            return new WorkflowExecutionResult
            {
                Success = false,
                ErrorMessage = "Workflow has no steps"
            };
        }

        var cts = new CancellationTokenSource();
        _runningWorkflows[workflowId] = cts;

        var stopwatch = Stopwatch.StartNew();
        var result = new WorkflowExecutionResult
        {
            Variables = initialVariables ?? new Dictionary<string, object?>()
        };

        try
        {
            var currentStepIndex = 0;

            while (currentStepIndex < steps.Count && !cts.Token.IsCancellationRequested)
            {
                var step = steps[currentStepIndex];
                result.Log.Add($"[{DateTime.Now:HH:mm:ss}] Executing step {currentStepIndex + 1}: {step.Name}");

                try
                {
                    var stepResult = await ExecuteStepAsync(step, result.Variables, cts.Token);

                    if (stepResult.Success)
                    {
                        result.StepsExecuted++;
                        result.Log.Add($"[{DateTime.Now:HH:mm:ss}] ✓ Step completed successfully");

                        // Store output variable if specified
                        if (!string.IsNullOrWhiteSpace(step.OutputVariable) && stepResult.Output != null)
                        {
                            result.Variables[step.OutputVariable] = stepResult.Output;
                        }

                        // Determine next step
                        if (step.NextStepOnSuccess == -1)
                        {
                            currentStepIndex++;
                        }
                        else if (step.NextStepOnSuccess == -2)
                        {
                            break; // End workflow
                        }
                        else
                        {
                            currentStepIndex = steps.FindIndex(s => s.Id == step.NextStepOnSuccess);
                            if (currentStepIndex == -1)
                            {
                                throw new InvalidOperationException($"Next step {step.NextStepOnSuccess} not found");
                            }
                        }
                    }
                    else
                    {
                        result.Log.Add($"[{DateTime.Now:HH:mm:ss}] ✗ Step failed: {stepResult.ErrorMessage}");

                        if (!step.ContinueOnError)
                        {
                            result.Success = false;
                            result.ErrorMessage = $"Step '{step.Name}' failed: {stepResult.ErrorMessage}";
                            break;
                        }

                        // Determine next step on failure
                        if (step.NextStepOnFailure == -1)
                        {
                            currentStepIndex++;
                        }
                        else if (step.NextStepOnFailure == -2)
                        {
                            break; // End workflow
                        }
                        else
                        {
                            currentStepIndex = steps.FindIndex(s => s.Id == step.NextStepOnFailure);
                            if (currentStepIndex == -1)
                            {
                                throw new InvalidOperationException($"Next step {step.NextStepOnFailure} not found");
                            }
                        }
                    }
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    result.Log.Add($"[{DateTime.Now:HH:mm:ss}] ✗ Error: {ex.Message}");

                    if (!step.ContinueOnError)
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Step '{step.Name}' error: {ex.Message}";
                        break;
                    }

                    currentStepIndex++;
                }
            }

            if (cts.Token.IsCancellationRequested)
            {
                result.Success = false;
                result.ErrorMessage = "Workflow was cancelled";
                result.Log.Add($"[{DateTime.Now:HH:mm:ss}] Workflow cancelled");
            }
            else if (result.ErrorMessage == null)
            {
                result.Success = true;
                result.Log.Add($"[{DateTime.Now:HH:mm:ss}] ✓ Workflow completed successfully");
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.Log.Add($"[{DateTime.Now:HH:mm:ss}] ✗ Workflow error: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            _runningWorkflows.Remove(workflowId);

            // Update statistics
            await _workflowService.UpdateWorkflowStatisticsAsync(workflowId, stopwatch.ElapsedMilliseconds);
        }

        return result;
    }

    public async Task<WorkflowExecutionResult> ExecuteWorkflowByNameAsync(
        string workflowName,
        Dictionary<string, object?>? initialVariables = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowName);

        var workflows = await _workflowService.GetAllWorkflowsAsync();
        var workflow = workflows.FirstOrDefault(w => w.Name.Equals(workflowName, StringComparison.OrdinalIgnoreCase));

        if (workflow == null)
        {
            return new WorkflowExecutionResult
            {
                Success = false,
                ErrorMessage = $"Workflow '{workflowName}' not found"
            };
        }

        return await ExecuteWorkflowAsync(workflow.Id, initialVariables);
    }

    public async Task<WorkflowExecutionResult> TestStepAsync(WorkflowStep step, Dictionary<string, object?> variables)
    {
        ArgumentNullException.ThrowIfNull(step);
        ArgumentNullException.ThrowIfNull(variables);

        var cts = new CancellationTokenSource();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await ExecuteStepAsync(step, variables, cts.Token);
            stopwatch.Stop();

            return new WorkflowExecutionResult
            {
                Success = result.Success,
                Duration = stopwatch.Elapsed,
                StepsExecuted = 1,
                ErrorMessage = result.ErrorMessage,
                Variables = variables,
                Log = new List<string> { result.Success ? "✓ Test passed" : $"✗ Test failed: {result.ErrorMessage}" }
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new WorkflowExecutionResult
            {
                Success = false,
                Duration = stopwatch.Elapsed,
                ErrorMessage = ex.Message,
                Log = new List<string> { $"✗ Test error: {ex.Message}" }
            };
        }
    }

    public async Task<(bool isValid, List<string> errors)> ValidateWorkflowAsync(int workflowId)
    {
        var errors = new List<string>();

        var workflow = await _workflowService.GetWorkflowAsync(workflowId);
        if (workflow == null)
        {
            errors.Add("Workflow not found");
            return (false, errors);
        }

        var steps = await _workflowService.GetWorkflowStepsAsync(workflowId);
        if (steps.Count == 0)
        {
            errors.Add("Workflow has no steps");
        }

        // Validate step references
        var stepIds = steps.Select(s => s.Id).ToHashSet();
        foreach (var step in steps)
        {
            if (step.NextStepOnSuccess > 0 && !stepIds.Contains(step.NextStepOnSuccess))
            {
                errors.Add($"Step '{step.Name}': Invalid NextStepOnSuccess reference ({step.NextStepOnSuccess})");
            }

            if (step.NextStepOnFailure > 0 && !stepIds.Contains(step.NextStepOnFailure))
            {
                errors.Add($"Step '{step.Name}': Invalid NextStepOnFailure reference ({step.NextStepOnFailure})");
            }

            if (step.StepType == WorkflowStepType.Command && string.IsNullOrWhiteSpace(step.Action))
            {
                errors.Add($"Step '{step.Name}': Command action is empty");
            }

            if (step.StepType == WorkflowStepType.Condition && string.IsNullOrWhiteSpace(step.Condition))
            {
                errors.Add($"Step '{step.Name}': Condition expression is empty");
            }
        }

        return (errors.Count == 0, errors);
    }

    public void CancelWorkflow(int workflowId)
    {
        if (_runningWorkflows.TryGetValue(workflowId, out var cts))
        {
            cts.Cancel();
        }
    }

    private async Task<(bool Success, string? ErrorMessage, object? Output)> ExecuteStepAsync(
        WorkflowStep step,
        Dictionary<string, object?> variables,
        CancellationToken cancellationToken)
    {
        try
        {
            return step.StepType switch
            {
                WorkflowStepType.Command => await ExecuteCommandStepAsync(step, variables, cancellationToken),
                WorkflowStepType.Condition => ExecuteConditionStep(step, variables),
                WorkflowStepType.Delay => await ExecuteDelayStepAsync(step, cancellationToken),
                WorkflowStepType.Variable => ExecuteVariableStep(step, variables),
                WorkflowStepType.Log => ExecuteLogStep(step, variables),
                WorkflowStepType.PluginAction => await ExecutePluginActionStepAsync(step, variables, cancellationToken),
                _ => (false, $"Unknown step type: {step.StepType}", null)
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    private async Task<(bool Success, string? ErrorMessage, object? Output)> ExecuteCommandStepAsync(
        WorkflowStep step,
        Dictionary<string, object?> variables,
        CancellationToken cancellationToken)
    {
        var command = SubstituteVariables(step.Action, variables);
        var result = await _executor.Execute(command);
        return (true, null, result);
    }

    private (bool Success, string? ErrorMessage, object? Output) ExecuteConditionStep(
        WorkflowStep step,
        Dictionary<string, object?> variables)
    {
        if (string.IsNullOrWhiteSpace(step.Condition))
        {
            return (false, "Condition expression is empty", null);
        }

        var condition = SubstituteVariables(step.Condition, variables);
        var result = EvaluateCondition(condition, variables);
        return (result, null, result);
    }

    private async Task<(bool Success, string? ErrorMessage, object? Output)> ExecuteDelayStepAsync(
        WorkflowStep step,
        CancellationToken cancellationToken)
    {
        await Task.Delay(step.DelayMs, cancellationToken);
        return (true, null, null);
    }

    private (bool Success, string? ErrorMessage, object? Output) ExecuteVariableStep(
        WorkflowStep step,
        Dictionary<string, object?> variables)
    {
        var value = SubstituteVariables(step.Action, variables);
        return (true, null, value);
    }

    private (bool Success, string? ErrorMessage, object? Output) ExecuteLogStep(
        WorkflowStep step,
        Dictionary<string, object?> variables)
    {
        var message = SubstituteVariables(step.Action, variables);
        return (true, null, message);
    }

    private async Task<(bool Success, string? ErrorMessage, object? Output)> ExecutePluginActionStepAsync(
        WorkflowStep step,
        Dictionary<string, object?> variables,
        CancellationToken cancellationToken)
    {
        // For plugin actions, use the executor with a formatted command
        var command = $"{step.PluginName}: {SubstituteVariables(step.Action, variables)}";
        var result = await _executor.Execute(command);
        return (true, null, result);
    }

    private string SubstituteVariables(string text, Dictionary<string, object?> variables)
    {
        return Regex.Replace(text, @"\{(\w+)\}", match =>
        {
            var varName = match.Groups[1].Value;
            return variables.TryGetValue(varName, out var value) ? value?.ToString() ?? string.Empty : match.Value;
        });
    }

    private bool EvaluateCondition(string condition, Dictionary<string, object?> variables)
    {
        // Simple condition evaluation (can be extended)
        // Supports: variable == value, variable != value, variable > value, etc.
        
        var operators = new[] { "==", "!=", ">=", "<=", ">", "<", "contains", "startswith", "endswith" };
        
        foreach (var op in operators)
        {
            if (condition.Contains(op, StringComparison.OrdinalIgnoreCase))
            {
                var parts = condition.Split(new[] { op }, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    var left = SubstituteVariables(parts[0].Trim(), variables);
                    var right = parts[1].Trim().Trim('"', '\'');

                    return op.ToLowerInvariant() switch
                    {
                        "==" => left.Equals(right, StringComparison.OrdinalIgnoreCase),
                        "!=" => !left.Equals(right, StringComparison.OrdinalIgnoreCase),
                        "contains" => left.Contains(right, StringComparison.OrdinalIgnoreCase),
                        "startswith" => left.StartsWith(right, StringComparison.OrdinalIgnoreCase),
                        "endswith" => left.EndsWith(right, StringComparison.OrdinalIgnoreCase),
                        ">" => double.TryParse(left, out var l1) && double.TryParse(right, out var r1) && l1 > r1,
                        "<" => double.TryParse(left, out var l2) && double.TryParse(right, out var r2) && l2 < r2,
                        ">=" => double.TryParse(left, out var l3) && double.TryParse(right, out var r3) && l3 >= r3,
                        "<=" => double.TryParse(left, out var l4) && double.TryParse(right, out var r4) && l4 <= r4,
                        _ => false
                    };
                }
            }
        }

        // If no operator found, treat as boolean variable
        var boolValue = SubstituteVariables(condition.Trim(), variables);
        return bool.TryParse(boolValue, out var result) && result;
    }
}
