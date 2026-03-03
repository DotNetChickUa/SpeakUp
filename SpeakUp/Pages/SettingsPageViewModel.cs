using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Models;
using SpeakUp.Services;

namespace SpeakUp.Pages;

public sealed partial class SettingsPageViewModel(ISettingsService settingsService) : ObservableObject
{
    private AppSettings _currentSettings = new();

    [ObservableProperty]
    public partial string ApiKey { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Model { get; set; } = "gpt-4";

    [ObservableProperty]
    public partial string? CustomEndpoint { get; set; }

    [ObservableProperty]
    public partial double Temperature { get; set; } = 0.7;

    [ObservableProperty]
    public partial int MaxTokens { get; set; } = 1000;

    [ObservableProperty]
    public partial bool UseOfflineRecognition { get; set; }

    [ObservableProperty]
    public partial string Language { get; set; } = "en-US";

    [ObservableProperty]
    public partial bool AutoExecute { get; set; } = true;

    [ObservableProperty]
    public partial bool ConfirmDestructiveCommands { get; set; } = true;

    [ObservableProperty]
    public partial bool EnableVoiceFeedback { get; set; }

    [ObservableProperty]
    public partial bool AutoLoadPlugins { get; set; } = true;

    [ObservableProperty]
    public partial bool ShowPluginStatus { get; set; } = true;

    [ObservableProperty]
    public partial bool DetailedLogging { get; set; }

    [ObservableProperty]
    public partial int MaxLogEntries { get; set; } = 100;

    [ObservableProperty]
    public partial bool AutoSaveHistory { get; set; } = true;

    [ObservableProperty]
    public partial string Theme { get; set; } = "System";

    [ObservableProperty]
    public partial bool IsSaving { get; set; }

    public async Task InitializeAsync()
    {
        _currentSettings = await settingsService.LoadSettingsAsync();
        LoadSettingsToProperties();
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        if (IsSaving)
        {
            return;
        }

        try
        {
            IsSaving = true;
            UpdateSettingsFromProperties();
            await settingsService.SaveSettingsAsync(_currentSettings);
            
            await Shell.Current.DisplayAlertAsync(
                "Settings Saved",
                "Your settings have been saved successfully.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                $"Failed to save settings: {ex.Message}",
                "OK");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task ResetSettingsAsync()
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            "Reset Settings",
            "Are you sure you want to reset all settings to defaults? This cannot be undone.",
            "Reset",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        try
        {
            await settingsService.ResetSettingsAsync();
            _currentSettings = new AppSettings();
            LoadSettingsToProperties();
            
            await Shell.Current.DisplayAlertAsync(
                "Settings Reset",
                "All settings have been reset to defaults.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync(
                "Error",
                $"Failed to reset settings: {ex.Message}",
                "OK");
        }
    }

    [RelayCommand]
    private static async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void LoadSettingsToProperties()
    {
        ApiKey = _currentSettings.AiProvider.ApiKey;
        Model = _currentSettings.AiProvider.Model;
        CustomEndpoint = _currentSettings.AiProvider.CustomEndpoint;
        Temperature = _currentSettings.AiProvider.Temperature;
        MaxTokens = _currentSettings.AiProvider.MaxTokens;

        UseOfflineRecognition = _currentSettings.Speech.UseOfflineRecognition;
        Language = _currentSettings.Speech.Language;
        AutoExecute = _currentSettings.Speech.AutoExecute;
        ConfirmDestructiveCommands = _currentSettings.Speech.ConfirmDestructiveCommands;
        EnableVoiceFeedback = _currentSettings.Speech.EnableVoiceFeedback;

        AutoLoadPlugins = _currentSettings.Plugins.AutoLoadPlugins;
        ShowPluginStatus = _currentSettings.Plugins.ShowPluginStatus;

        DetailedLogging = _currentSettings.General.DetailedLogging;
        MaxLogEntries = _currentSettings.General.MaxLogEntries;
        AutoSaveHistory = _currentSettings.General.AutoSaveHistory;
        Theme = _currentSettings.General.Theme;
    }

    private void UpdateSettingsFromProperties()
    {
        _currentSettings.AiProvider.ApiKey = ApiKey;
        _currentSettings.AiProvider.Model = Model;
        _currentSettings.AiProvider.CustomEndpoint = CustomEndpoint;
        _currentSettings.AiProvider.Temperature = Temperature;
        _currentSettings.AiProvider.MaxTokens = MaxTokens;

        _currentSettings.Speech.UseOfflineRecognition = UseOfflineRecognition;
        _currentSettings.Speech.Language = Language;
        _currentSettings.Speech.AutoExecute = AutoExecute;
        _currentSettings.Speech.ConfirmDestructiveCommands = ConfirmDestructiveCommands;
        _currentSettings.Speech.EnableVoiceFeedback = EnableVoiceFeedback;

        _currentSettings.Plugins.AutoLoadPlugins = AutoLoadPlugins;
        _currentSettings.Plugins.ShowPluginStatus = ShowPluginStatus;

        _currentSettings.General.DetailedLogging = DetailedLogging;
        _currentSettings.General.MaxLogEntries = MaxLogEntries;
        _currentSettings.General.AutoSaveHistory = AutoSaveHistory;
        _currentSettings.General.Theme = Theme;
    }
}
