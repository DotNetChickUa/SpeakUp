namespace SpeakUp.Models;

/// <summary>
/// Application settings data model
/// </summary>
public sealed class AppSettings
{
    /// <summary>
    /// AI provider settings
    /// </summary>
    public AiProviderSettings AiProvider { get; set; } = new();

    /// <summary>
    /// Speech recognition settings
    /// </summary>
    public SpeechSettings Speech { get; set; } = new();

    /// <summary>
    /// Plugin configuration settings
    /// </summary>
    public PluginSettings Plugins { get; set; } = new();

    /// <summary>
    /// General application settings
    /// </summary>
    public GeneralSettings General { get; set; } = new();
}

/// <summary>
/// AI provider configuration
/// </summary>
public sealed class AiProviderSettings
{
    /// <summary>
    /// OpenAI API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// AI model to use (e.g., gpt-4, gpt-3.5-turbo)
    /// </summary>
    public string Model { get; set; } = "gpt-4";

    /// <summary>
    /// Custom API endpoint (optional)
    /// </summary>
    public string? CustomEndpoint { get; set; }

    /// <summary>
    /// Temperature for AI responses (0.0 - 2.0)
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Maximum tokens for AI responses
    /// </summary>
    public int MaxTokens { get; set; } = 1000;
}

/// <summary>
/// Speech recognition configuration
/// </summary>
public sealed class SpeechSettings
{
    /// <summary>
    /// Use offline speech recognition by default
    /// </summary>
    public bool UseOfflineRecognition { get; set; }

    /// <summary>
    /// Speech recognition language (e.g., en-US, es-ES)
    /// </summary>
    public string Language { get; set; } = "en-US";

    /// <summary>
    /// Automatically execute recognized commands
    /// </summary>
    public bool AutoExecute { get; set; } = true;

    /// <summary>
    /// Confirmation prompt for destructive commands
    /// </summary>
    public bool ConfirmDestructiveCommands { get; set; } = true;

    /// <summary>
    /// Enable voice feedback (text-to-speech)
    /// </summary>
    public bool EnableVoiceFeedback { get; set; }
}

/// <summary>
/// Plugin configuration
/// </summary>
public sealed class PluginSettings
{
    /// <summary>
    /// Enabled plugin names
    /// </summary>
    public HashSet<string> EnabledPlugins { get; set; } = [];

    /// <summary>
    /// Automatically load all available plugins
    /// </summary>
    public bool AutoLoadPlugins { get; set; } = true;

    /// <summary>
    /// Show plugin status on main page
    /// </summary>
    public bool ShowPluginStatus { get; set; } = true;
}

/// <summary>
/// General application settings
/// </summary>
public sealed class GeneralSettings
{
    /// <summary>
    /// Enable detailed logging
    /// </summary>
    public bool DetailedLogging { get; set; }

    /// <summary>
    /// Maximum number of log entries to display
    /// </summary>
    public int MaxLogEntries { get; set; } = 100;

    /// <summary>
    /// Auto-save conversation history
    /// </summary>
    public bool AutoSaveHistory { get; set; } = true;

    /// <summary>
    /// Theme preference (System, Light, Dark)
    /// </summary>
    public string Theme { get; set; } = "System";
}
