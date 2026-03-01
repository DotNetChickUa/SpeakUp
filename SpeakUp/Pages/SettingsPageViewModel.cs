using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Models;
using SpeakUp.Services;

namespace SpeakUp.Pages;

public sealed partial class SettingsPageViewModel(ISettingsService settingsService) : ObservableObject
{
    private AppSettings _currentSettings = new();

    [ObservableProperty]
    private string _apiKey = string.Empty;

    [ObservableProperty]
    private string _model = "gpt-4";

    [ObservableProperty]
    private string? _customEndpoint;

    [ObservableProperty]
    private double _temperature = 0.7;

    [ObservableProperty]
    private int _maxTokens = 1000;

    [ObservableProperty]
    private bool _useOfflineRecognition;

    [ObservableProperty]
    private string _language = "en-US";

    [ObservableProperty]
    private bool _autoExecute = true;

    [ObservableProperty]
    private bool _confirmDestructiveCommands = true;

    [ObservableProperty]
    private bool _enableVoiceFeedback;

    [ObservableProperty]
    private bool _autoLoadPlugins = true;

    [ObservableProperty]
    private bool _showPluginStatus = true;

    [ObservableProperty]
    private bool _detailedLogging;

    [ObservableProperty]
    private int _maxLogEntries = 100;

    [ObservableProperty]
    private bool _autoSaveHistory = true;

    [ObservableProperty]
    private string _theme = "System";

    [ObservableProperty]
    private bool _isSaving;

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
            
            await Shell.Current.DisplayAlert(
                "Settings Saved",
                "Your settings have been saved successfully.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
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
        var confirm = await Shell.Current.DisplayAlert(
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
            
            await Shell.Current.DisplayAlert(
                "Settings Reset",
                "All settings have been reset to defaults.",
                "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
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
