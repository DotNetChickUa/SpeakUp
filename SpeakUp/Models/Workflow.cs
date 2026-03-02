using SQLite;

namespace SpeakUp.Models;

/// <summary>
/// Represents a workflow/automation
/// </summary>
[Table("Workflows")]
public sealed class Workflow
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Workflow name
    /// </summary>
    [Indexed]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Workflow description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the workflow is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Icon emoji for the workflow
    /// </summary>
    public string Icon { get; set; } = "⚡";

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last modified timestamp
    /// </summary>
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last execution timestamp
    /// </summary>
    public DateTime? LastExecutedAt { get; set; }

    /// <summary>
    /// Number of times executed
    /// </summary>
    public int ExecutionCount { get; set; }

    /// <summary>
    /// Average execution duration in milliseconds
    /// </summary>
    public long AverageDurationMs { get; set; }

    /// <summary>
    /// Category for organization
    /// </summary>
    public string Category { get; set; } = "General";

    /// <summary>
    /// Tags for searching (comma-separated)
    /// </summary>
    public string? Tags { get; set; }
}

/// <summary>
/// Represents a step in a workflow
/// </summary>
[Table("WorkflowSteps")]
public sealed class WorkflowStep
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Parent workflow ID
    /// </summary>
    [Indexed]
    public int WorkflowId { get; set; }

    /// <summary>
    /// Step order in the workflow
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Step type
    /// </summary>
    public WorkflowStepType StepType { get; set; }

    /// <summary>
    /// Step name/description
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Command or action to execute (JSON for complex actions)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Condition expression (for conditional steps)
    /// </summary>
    public string? Condition { get; set; }

    /// <summary>
    /// Delay in milliseconds (for delay steps)
    /// </summary>
    public int DelayMs { get; set; }

    /// <summary>
    /// Target plugin name (if specific plugin action)
    /// </summary>
    public string? PluginName { get; set; }

    /// <summary>
    /// Whether to continue on error
    /// </summary>
    public bool ContinueOnError { get; set; }

    /// <summary>
    /// Variable name to store result
    /// </summary>
    public string? OutputVariable { get; set; }

    /// <summary>
    /// Next step ID on success (-1 for next in order)
    /// </summary>
    public int NextStepOnSuccess { get; set; } = -1;

    /// <summary>
    /// Next step ID on failure (-1 for next in order, -2 to end)
    /// </summary>
    public int NextStepOnFailure { get; set; } = -1;
}

/// <summary>
/// Represents a trigger that activates a workflow
/// </summary>
[Table("WorkflowTriggers")]
public sealed class WorkflowTrigger
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Parent workflow ID
    /// </summary>
    [Indexed]
    public int WorkflowId { get; set; }

    /// <summary>
    /// Trigger type
    /// </summary>
    public WorkflowTriggerType TriggerType { get; set; }

    /// <summary>
    /// Voice keyword(s) that trigger the workflow (comma-separated)
    /// </summary>
    public string? VoiceKeywords { get; set; }

    /// <summary>
    /// Schedule cron expression (for scheduled triggers)
    /// </summary>
    public string? CronExpression { get; set; }

    /// <summary>
    /// Whether the trigger is enabled
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Last triggered timestamp
    /// </summary>
    public DateTime? LastTriggeredAt { get; set; }
}

/// <summary>
/// Workflow step types
/// </summary>
public enum WorkflowStepType
{
    /// <summary>
    /// Execute a command
    /// </summary>
    Command,

    /// <summary>
    /// Conditional branch
    /// </summary>
    Condition,

    /// <summary>
    /// Delay/wait
    /// </summary>
    Delay,

    /// <summary>
    /// Variable assignment
    /// </summary>
    Variable,

    /// <summary>
    /// Loop iteration
    /// </summary>
    Loop,

    /// <summary>
    /// Plugin action
    /// </summary>
    PluginAction,

    /// <summary>
    /// User input prompt
    /// </summary>
    UserInput,

    /// <summary>
    /// Log message
    /// </summary>
    Log
}

/// <summary>
/// Workflow trigger types
/// </summary>
public enum WorkflowTriggerType
{
    /// <summary>
    /// Manual execution only
    /// </summary>
    Manual,

    /// <summary>
    /// Voice command trigger
    /// </summary>
    Voice,

    /// <summary>
    /// Scheduled execution
    /// </summary>
    Schedule,

    /// <summary>
    /// Event-based trigger
    /// </summary>
    Event,

    /// <summary>
    /// File system watcher
    /// </summary>
    FileSystem,

    /// <summary>
    /// API webhook
    /// </summary>
    Webhook
}

/// <summary>
/// Workflow execution result
/// </summary>
public sealed class WorkflowExecutionResult
{
    /// <summary>
    /// Whether execution was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Execution duration
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Number of steps executed
    /// </summary>
    public int StepsExecuted { get; set; }

    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Output/result data
    /// </summary>
    public Dictionary<string, object?> Variables { get; set; } = new();

    /// <summary>
    /// Execution log
    /// </summary>
    public List<string> Log { get; set; } = new();
}
