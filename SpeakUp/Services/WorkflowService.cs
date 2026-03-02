using SQLite;
using SpeakUp.Models;

namespace SpeakUp.Services;

/// <summary>
/// Service for workflow persistence and management
/// </summary>
public interface IWorkflowService
{
    /// <summary>
    /// Initialize the database
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Get all workflows
    /// </summary>
    Task<List<Workflow>> GetAllWorkflowsAsync();

    /// <summary>
    /// Get a workflow by ID
    /// </summary>
    Task<Workflow?> GetWorkflowAsync(int id);

    /// <summary>
    /// Create or update a workflow
    /// </summary>
    Task<int> SaveWorkflowAsync(Workflow workflow);

    /// <summary>
    /// Delete a workflow
    /// </summary>
    Task DeleteWorkflowAsync(int id);

    /// <summary>
    /// Get steps for a workflow
    /// </summary>
    Task<List<WorkflowStep>> GetWorkflowStepsAsync(int workflowId);

    /// <summary>
    /// Save a workflow step
    /// </summary>
    Task<int> SaveWorkflowStepAsync(WorkflowStep step);

    /// <summary>
    /// Delete a workflow step
    /// </summary>
    Task DeleteWorkflowStepAsync(int stepId);

    /// <summary>
    /// Get triggers for a workflow
    /// </summary>
    Task<List<WorkflowTrigger>> GetWorkflowTriggersAsync(int workflowId);

    /// <summary>
    /// Save a workflow trigger
    /// </summary>
    Task<int> SaveWorkflowTriggerAsync(WorkflowTrigger trigger);

    /// <summary>
    /// Delete a workflow trigger
    /// </summary>
    Task DeleteWorkflowTriggerAsync(int triggerId);

    /// <summary>
    /// Get workflows by category
    /// </summary>
    Task<List<Workflow>> GetWorkflowsByCategoryAsync(string category);

    /// <summary>
    /// Search workflows
    /// </summary>
    Task<List<Workflow>> SearchWorkflowsAsync(string searchText);

    /// <summary>
    /// Get enabled workflows with voice triggers
    /// </summary>
    Task<List<(Workflow workflow, WorkflowTrigger trigger)>> GetVoiceTriggeredWorkflowsAsync();

    /// <summary>
    /// Update workflow execution statistics
    /// </summary>
    Task UpdateWorkflowStatisticsAsync(int workflowId, long durationMs);
}

/// <summary>
/// SQLite implementation of workflow service
/// </summary>
internal sealed class WorkflowService : IWorkflowService
{
    private readonly SQLiteAsyncConnection _database;
    private bool _isInitialized;

    public WorkflowService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "workflows.db3");
        _database = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _database.CreateTableAsync<Workflow>();
        await _database.CreateTableAsync<WorkflowStep>();
        await _database.CreateTableAsync<WorkflowTrigger>();
        _isInitialized = true;
    }

    public async Task<List<Workflow>> GetAllWorkflowsAsync()
    {
        await InitializeAsync();
        return await _database.Table<Workflow>()
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<Workflow?> GetWorkflowAsync(int id)
    {
        await InitializeAsync();
        return await _database.Table<Workflow>()
            .Where(w => w.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveWorkflowAsync(Workflow workflow)
    {
        ArgumentNullException.ThrowIfNull(workflow);
        await InitializeAsync();

        workflow.ModifiedAt = DateTime.UtcNow;

        if (workflow.Id == 0)
        {
            workflow.CreatedAt = DateTime.UtcNow;
            await _database.InsertAsync(workflow);
        }
        else
        {
            await _database.UpdateAsync(workflow);
        }

        return workflow.Id;
    }

    public async Task DeleteWorkflowAsync(int id)
    {
        await InitializeAsync();

        // Delete steps and triggers first
        await _database.ExecuteAsync("DELETE FROM WorkflowSteps WHERE WorkflowId = ?", id);
        await _database.ExecuteAsync("DELETE FROM WorkflowTriggers WHERE WorkflowId = ?", id);

        // Delete workflow
        await _database.ExecuteAsync("DELETE FROM Workflows WHERE Id = ?", id);
    }

    public async Task<List<WorkflowStep>> GetWorkflowStepsAsync(int workflowId)
    {
        await InitializeAsync();
        return await _database.Table<WorkflowStep>()
            .Where(s => s.WorkflowId == workflowId)
            .OrderBy(s => s.Order)
            .ToListAsync();
    }

    public async Task<int> SaveWorkflowStepAsync(WorkflowStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        await InitializeAsync();

        if (step.Id == 0)
        {
            await _database.InsertAsync(step);
        }
        else
        {
            await _database.UpdateAsync(step);
        }

        return step.Id;
    }

    public async Task DeleteWorkflowStepAsync(int stepId)
    {
        await InitializeAsync();
        await _database.ExecuteAsync("DELETE FROM WorkflowSteps WHERE Id = ?", stepId);
    }

    public async Task<List<WorkflowTrigger>> GetWorkflowTriggersAsync(int workflowId)
    {
        await InitializeAsync();
        return await _database.Table<WorkflowTrigger>()
            .Where(t => t.WorkflowId == workflowId)
            .ToListAsync();
    }

    public async Task<int> SaveWorkflowTriggerAsync(WorkflowTrigger trigger)
    {
        ArgumentNullException.ThrowIfNull(trigger);
        await InitializeAsync();

        if (trigger.Id == 0)
        {
            await _database.InsertAsync(trigger);
        }
        else
        {
            await _database.UpdateAsync(trigger);
        }

        return trigger.Id;
    }

    public async Task DeleteWorkflowTriggerAsync(int triggerId)
    {
        await InitializeAsync();
        await _database.ExecuteAsync("DELETE FROM WorkflowTriggers WHERE Id = ?", triggerId);
    }

    public async Task<List<Workflow>> GetWorkflowsByCategoryAsync(string category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        await InitializeAsync();

        return await _database.Table<Workflow>()
            .Where(w => w.Category == category)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<List<Workflow>> SearchWorkflowsAsync(string searchText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchText);
        await InitializeAsync();

        var searchLower = searchText.ToLowerInvariant();
        return await _database.Table<Workflow>()
            .Where(w => w.Name.ToLower().Contains(searchLower) ||
                       (w.Description != null && w.Description.ToLower().Contains(searchLower)) ||
                       (w.Tags != null && w.Tags.ToLower().Contains(searchLower)))
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<List<(Workflow workflow, WorkflowTrigger trigger)>> GetVoiceTriggeredWorkflowsAsync()
    {
        await InitializeAsync();

        var workflows = await _database.Table<Workflow>()
            .Where(w => w.IsEnabled)
            .ToListAsync();

        var triggers = await _database.Table<WorkflowTrigger>()
            .Where(t => t.IsEnabled && t.TriggerType == WorkflowTriggerType.Voice)
            .ToListAsync();

        return (from w in workflows
                join t in triggers on w.Id equals t.WorkflowId
                select (w, t)).ToList();
    }

    public async Task UpdateWorkflowStatisticsAsync(int workflowId, long durationMs)
    {
        await InitializeAsync();

        var workflow = await GetWorkflowAsync(workflowId);
        if (workflow == null)
        {
            return;
        }

        workflow.ExecutionCount++;
        workflow.LastExecutedAt = DateTime.UtcNow;

        // Calculate rolling average
        if (workflow.ExecutionCount == 1)
        {
            workflow.AverageDurationMs = durationMs;
        }
        else
        {
            workflow.AverageDurationMs = (workflow.AverageDurationMs * (workflow.ExecutionCount - 1) + durationMs) / workflow.ExecutionCount;
        }

        await _database.UpdateAsync(workflow);
    }
}
