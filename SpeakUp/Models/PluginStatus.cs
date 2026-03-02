namespace SpeakUp.Models;

/// <summary>
/// Plugin status information
/// </summary>
public class PluginStatus
{
    /// <summary>
    /// Plugin name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Plugin category (Communication, Storage, System, etc.)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Whether the plugin is currently loaded
    /// </summary>
    public bool IsLoaded { get; set; }

    /// <summary>
    /// Number of commands available in the plugin
    /// </summary>
    public int CommandCount { get; set; }

    /// <summary>
    /// Plugin version
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Status icon emoji
    /// </summary>
    public string Icon => IsLoaded ? "✅" : "⏸️";

    /// <summary>
    /// Plugin description
    /// </summary>
    public string? Description { get; set; }
}
