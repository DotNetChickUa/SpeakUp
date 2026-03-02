using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Models;
using SpeakUp.Services;
using System.Collections.ObjectModel;

namespace SpeakUp.Pages;

public sealed partial class WorkflowEditorPageViewModel : ObservableObject
{
    private readonly IWorkflowService _workflowService;
    private readonly IWorkflowExecutionService _executionService;
    private readonly IErrorHandlingService _errorHandler;

    [ObservableProperty]
    private int _workflowId;

    [ObservableProperty]
    private string _workflowName = string.Empty;

    [ObservableProperty]
    private string _workflowDescription = string.Empty;

    [ObservableProperty]
    private string _workflowIcon = "⚡";

    [ObservableProperty]
    private string _workflowCategory = "General";

    [ObservableProperty]
    private bool _isWorkflowEnabled = true;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isTestRunning;

    [ObservableProperty]
    private WorkflowStep? _selectedStep;

    public ObservableCollection<WorkflowStep> Steps { get; } = [];
    public ObservableCollection<WorkflowTrigger> Triggers { get; } = [];

    public ObservableCollection<string> Categories { get; } = new()
    {
        "General",
        "Communication",
        "Automation",
        "System",
        "Testing",
        "Custom"
    };

    public ObservableCollection<string> StepTypeOptions { get; } = new()
    {
        "Command",
        "Condition",
        "Delay",
        "Variable",
        "Loop",
        "PluginAction",
        "UserInput",
        "Log"
    };

    public WorkflowEditorPageViewModel(
        IWorkflowService workflowService,
        IWorkflowExecutionService executionService,
        IErrorHandlingService errorHandler)
    {
        _workflowService = workflowService;
        _executionService = executionService;
        _errorHandler = errorHandler;
    }

    public async Task InitializeAsync(int workflowId)
    {
        WorkflowId = workflowId;
        await LoadWorkflowAsync();
    }

    [RelayCommand]
    private async Task LoadWorkflowAsync()
    {
        if (IsLoading || WorkflowId == 0)
        {
            return;
        }

        try
        {
            IsLoading = true;

            var workflow = await _workflowService.GetWorkflowAsync(WorkflowId);
            if (workflow != null)
            {
                WorkflowName = workflow.Name;
                WorkflowDescription = workflow.Description ?? string.Empty;
                WorkflowIcon = workflow.Icon;
                WorkflowCategory = workflow.Category;
                IsWorkflowEnabled = workflow.IsEnabled;
            }

            var steps = await _workflowService.GetWorkflowStepsAsync(WorkflowId);
            Steps.Clear();
            foreach (var step in steps)
            {
                Steps.Add(step);
            }

            var triggers = await _workflowService.GetWorkflowTriggersAsync(WorkflowId);
            Triggers.Clear();
            foreach (var trigger in triggers)
            {
                Triggers.Add(trigger);
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Loading workflow");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveWorkflowAsync()
    {
        if (string.IsNullOrWhiteSpace(WorkflowName))
        {
            await _errorHandler.ShowErrorAsync("Validation Error", "Workflow name is required");
            return;
        }

        try
        {
            IsLoading = true;

            var workflow = new Workflow
            {
                Id = WorkflowId,
                Name = WorkflowName,
                Description = WorkflowDescription,
                Icon = WorkflowIcon,
                Category = WorkflowCategory,
                IsEnabled = IsWorkflowEnabled
            };

            await _workflowService.SaveWorkflowAsync(workflow);

            // Save all steps with updated order
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].Order = i;
                Steps[i].WorkflowId = WorkflowId;
                await _workflowService.SaveWorkflowStepAsync(Steps[i]);
            }

            // Save all triggers
            foreach (var trigger in Triggers)
            {
                trigger.WorkflowId = WorkflowId;
                await _workflowService.SaveWorkflowTriggerAsync(trigger);
            }

            await Shell.Current.DisplayAlert("Success", "Workflow saved successfully", "OK");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Saving workflow");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddStepAsync()
    {
        var stepName = await Shell.Current.DisplayPromptAsync(
            "New Step",
            "Enter step name:",
            "Add",
            "Cancel",
            "Step " + (Steps.Count + 1));

        if (string.IsNullOrWhiteSpace(stepName))
        {
            return;
        }

        var step = new WorkflowStep
        {
            WorkflowId = WorkflowId,
            Order = Steps.Count,
            Name = stepName,
            StepType = WorkflowStepType.Command,
            Action = string.Empty
        };

        Steps.Add(step);
        SelectedStep = step;
    }

    [RelayCommand]
    private async Task EditStepAsync(WorkflowStep step)
    {
        if (step == null)
        {
            return;
        }

        SelectedStep = step;
        
        // Navigate to step details (could be a separate page or a sheet)
        await Shell.Current.DisplayAlert(
            "Edit Step",
            $"Step editor for '{step.Name}'\n\nType: {step.StepType}\nAction: {step.Action}",
            "OK");
    }

    [RelayCommand]
    private async Task DeleteStepAsync(WorkflowStep step)
    {
        if (step == null)
        {
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Delete Step",
            $"Delete step '{step.Name}'?",
            "Delete",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        try
        {
            if (step.Id > 0)
            {
                await _workflowService.DeleteWorkflowStepAsync(step.Id);
            }

            Steps.Remove(step);
            
            // Update order
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].Order = i;
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, $"Deleting step '{step.Name}'");
        }
    }

    [RelayCommand]
    private void MoveStepUp(WorkflowStep step)
    {
        if (step == null)
        {
            return;
        }

        var index = Steps.IndexOf(step);
        if (index > 0)
        {
            Steps.Move(index, index - 1);
            
            // Update order
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].Order = i;
            }
        }
    }

    [RelayCommand]
    private void MoveStepDown(WorkflowStep step)
    {
        if (step == null)
        {
            return;
        }

        var index = Steps.IndexOf(step);
        if (index < Steps.Count - 1)
        {
            Steps.Move(index, index + 1);
            
            // Update order
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].Order = i;
            }
        }
    }

    [RelayCommand]
    private async Task AddTriggerAsync()
    {
        var trigger = new WorkflowTrigger
        {
            WorkflowId = WorkflowId,
            TriggerType = WorkflowTriggerType.Manual,
            IsEnabled = true
        };

        Triggers.Add(trigger);

        await Shell.Current.DisplayAlert(
            "Trigger Added",
            "Configure the trigger properties in the Triggers section",
            "OK");
    }

    [RelayCommand]
    private async Task DeleteTriggerAsync(WorkflowTrigger trigger)
    {
        if (trigger == null)
        {
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Delete Trigger",
            "Delete this trigger?",
            "Delete",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        try
        {
            if (trigger.Id > 0)
            {
                await _workflowService.DeleteWorkflowTriggerAsync(trigger.Id);
            }

            Triggers.Remove(trigger);
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Deleting trigger");
        }
    }

    [RelayCommand]
    private async Task ValidateWorkflowAsync()
    {
        try
        {
            // Save first
            await SaveWorkflowAsync();

            var (isValid, errors) = await _executionService.ValidateWorkflowAsync(WorkflowId);

            if (isValid)
            {
                await Shell.Current.DisplayAlert(
                    "✓ Valid",
                    "Workflow is valid and ready to execute",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "✗ Invalid",
                    $"Workflow has errors:\n\n{string.Join("\n", errors)}",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Validating workflow");
        }
    }

    [RelayCommand]
    private async Task TestWorkflowAsync()
    {
        if (IsTestRunning)
        {
            return;
        }

        try
        {
            IsTestRunning = true;

            // Save first
            await SaveWorkflowAsync();

            var result = await _executionService.ExecuteWorkflowAsync(WorkflowId);

            var message = result.Success
                ? $"✓ Test passed!\n\nDuration: {result.Duration.TotalSeconds:F2}s\nSteps: {result.StepsExecuted}\n\nLog:\n{string.Join("\n", result.Log.TakeLast(5))}"
                : $"✗ Test failed!\n\n{result.ErrorMessage}\n\nSteps completed: {result.StepsExecuted}\n\nLog:\n{string.Join("\n", result.Log.TakeLast(5))}";

            await Shell.Current.DisplayAlert(
                result.Success ? "Test Successful" : "Test Failed",
                message,
                "OK");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Testing workflow");
        }
        finally
        {
            IsTestRunning = false;
        }
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        var hasChanges = Steps.Any() || Triggers.Any();
        
        if (hasChanges)
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Unsaved Changes",
                "Save changes before leaving?",
                "Save",
                "Discard");

            if (confirm)
            {
                await SaveWorkflowAsync();
            }
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task QuickAddCommandStepAsync()
    {
        var command = await Shell.Current.DisplayPromptAsync(
            "Command Step",
            "Enter command to execute:",
            "Add",
            "Cancel");

        if (string.IsNullOrWhiteSpace(command))
        {
            return;
        }

        var step = new WorkflowStep
        {
            WorkflowId = WorkflowId,
            Order = Steps.Count,
            Name = $"Execute: {command.Substring(0, Math.Min(30, command.Length))}",
            StepType = WorkflowStepType.Command,
            Action = command
        };

        Steps.Add(step);
    }

    [RelayCommand]
    private async Task QuickAddDelayStepAsync()
    {
        var delayText = await Shell.Current.DisplayPromptAsync(
            "Delay Step",
            "Enter delay in milliseconds:",
            "Add",
            "Cancel",
            "1000");

        if (string.IsNullOrWhiteSpace(delayText) || !int.TryParse(delayText, out var delayMs))
        {
            return;
        }

        var step = new WorkflowStep
        {
            WorkflowId = WorkflowId,
            Order = Steps.Count,
            Name = $"Wait {delayMs}ms",
            StepType = WorkflowStepType.Delay,
            DelayMs = delayMs
        };

        Steps.Add(step);
    }

    [RelayCommand]
    private async Task QuickAddLogStepAsync()
    {
        var message = await Shell.Current.DisplayPromptAsync(
            "Log Step",
            "Enter message to log:",
            "Add",
            "Cancel");

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var step = new WorkflowStep
        {
            WorkflowId = WorkflowId,
            Order = Steps.Count,
            Name = $"Log: {message.Substring(0, Math.Min(30, message.Length))}",
            StepType = WorkflowStepType.Log,
            Action = message
        };

        Steps.Add(step);
    }
}
