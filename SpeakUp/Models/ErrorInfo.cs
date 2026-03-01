namespace SpeakUp.Models;

/// <summary>
/// Error information for display
/// </summary>
public sealed class ErrorInfo
{
    /// <summary>
    /// Error title
    /// </summary>
    public string Title { get; set; } = "Error";

    /// <summary>
    /// Error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Error severity level
    /// </summary>
    public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;

    /// <summary>
    /// Timestamp when error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Exception details (if available)
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Context information
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// Suggested action to resolve
    /// </summary>
    public string? SuggestedAction { get; set; }

    /// <summary>
    /// Error icon based on severity
    /// </summary>
    public string Icon => Severity switch
    {
        ErrorSeverity.Warning => "⚠️",
        ErrorSeverity.Error => "❌",
        ErrorSeverity.Critical => "🔴",
        ErrorSeverity.Info => "ℹ️",
        _ => "❓"
    };

    /// <summary>
    /// Display message with context
    /// </summary>
    public string DisplayMessage
    {
        get
        {
            var msg = Message;
            if (!string.IsNullOrWhiteSpace(Context))
            {
                msg = $"{Context}: {msg}";
            }
            return msg;
        }
    }
}

/// <summary>
/// Error severity levels
/// </summary>
public enum ErrorSeverity
{
    Info,
    Warning,
    Error,
    Critical
}
