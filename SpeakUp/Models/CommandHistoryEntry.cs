using SQLite;

namespace SpeakUp.Models;

/// <summary>
/// Represents a command execution in history
/// </summary>
[Table("CommandHistory")]
public sealed class CommandHistoryEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Session identifier for grouping related commands
    /// </summary>
    [Indexed]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Recognized speech text or command input
    /// </summary>
    public string Command { get; set; } = string.Empty;

    /// <summary>
    /// AI agent execution result
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// Execution status (Success, Error, Cancelled)
    /// </summary>
    public ExecutionStatus Status { get; set; }

    /// <summary>
    /// Error message if execution failed
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Timestamp when command was executed
    /// </summary>
    [Indexed]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Execution duration in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Speech recognition mode used (Online/Offline)
    /// </summary>
    public string RecognitionMode { get; set; } = string.Empty;

    /// <summary>
    /// Tags for categorization and search
    /// </summary>
    public string? Tags { get; set; }
}

/// <summary>
/// Command execution status
/// </summary>
public enum ExecutionStatus
{
    Success,
    Error,
    Cancelled,
    Pending
}

/// <summary>
/// Represents a conversation session
/// </summary>
[Table("Sessions")]
public sealed class SessionInfo
{
    [PrimaryKey]
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// User-friendly session name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Session start time
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Session end time
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Number of commands in session
    /// </summary>
    public int CommandCount { get; set; }

    /// <summary>
    /// Optional notes or description
    /// </summary>
    public string? Notes { get; set; }
}
