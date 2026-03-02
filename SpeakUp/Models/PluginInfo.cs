namespace SpeakUp.Models;

/// <summary>
/// Extended plugin information for management
/// </summary>
public sealed class PluginInfo : PluginStatus
{
    /// <summary>
    /// Whether the plugin is enabled in settings
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Plugin file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Plugin size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Last modified date
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Combined status text
    /// </summary>
    public string StatusText => IsEnabled ? (IsLoaded ? "✅ Active" : "⏳ Loading") : "⏸️ Disabled";
}
