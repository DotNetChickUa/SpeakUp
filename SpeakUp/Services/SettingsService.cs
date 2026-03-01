using System.Text.Json;
using SpeakUp.Models;

namespace SpeakUp.Services;

/// <summary>
/// Service for managing application settings persistence
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Load settings from storage
    /// </summary>
    Task<AppSettings> LoadSettingsAsync();

    /// <summary>
    /// Save settings to storage
    /// </summary>
    Task SaveSettingsAsync(AppSettings settings);

    /// <summary>
    /// Reset settings to defaults
    /// </summary>
    Task ResetSettingsAsync();
}

/// <summary>
/// Implementation of settings service using Preferences API
/// </summary>
internal sealed class SettingsService : ISettingsService
{
    private const string SettingsKey = "AppSettings";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public async Task<AppSettings> LoadSettingsAsync()
    {
        await Task.CompletedTask;

        var json = Preferences.Default.Get(SettingsKey, string.Empty);
        
        if (string.IsNullOrWhiteSpace(json))
        {
            return new AppSettings();
        }

        try
        {
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }
        catch (JsonException)
        {
            return new AppSettings();
        }
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        await Task.CompletedTask;

        var json = JsonSerializer.Serialize(settings, JsonOptions);
        Preferences.Default.Set(SettingsKey, json);
    }

    public async Task ResetSettingsAsync()
    {
        await Task.CompletedTask;
        
        Preferences.Default.Remove(SettingsKey);
    }
}
