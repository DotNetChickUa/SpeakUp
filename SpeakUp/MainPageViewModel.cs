using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Executor;
using SpeakUp.Services;

namespace SpeakUp;

public sealed partial class MainPageViewModel(
    IExecutor executor,
    ISettingsService settingsService,
    [FromKeyedServices(nameof(SpeechToTextImplementation))] ISpeechToText onlineSpeechToText,
    [FromKeyedServices(nameof(OfflineSpeechToTextImplementation))] ISpeechToText offlineSpeechToText) : ObservableObject, IDisposable
{
    private readonly ISettingsService _settingsService = settingsService;
    private bool _isSubscribed;
    private bool _isInitialized;
    private bool _autoExecuteCommands = true;
    private CultureInfo _speechCulture = CultureInfo.CurrentCulture;

    public ObservableCollection<string> Logs { get; set; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsListening))]
    [NotifyPropertyChangedFor(nameof(IsStopped))]
    public partial SpeechToTextState State { get; set; } = SpeechToTextState.Stopped;

    [ObservableProperty]
    public partial string? RecognitionResult { get; set; }

    [ObservableProperty]
    public partial bool IsOfflineSpeechToText { get; set; }

    public bool IsListening => State == SpeechToTextState.Listening;

    public bool IsStopped => State == SpeechToTextState.Stopped;

    private ISpeechToText SpeechToText => IsOfflineSpeechToText ? offlineSpeechToText : onlineSpeechToText;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        var settings = await _settingsService.LoadSettingsAsync();

        IsOfflineSpeechToText = settings.Speech.UseOfflineRecognition;
        _autoExecuteCommands = settings.Speech.AutoExecute;
        _speechCulture = ResolveCulture(settings.Speech.Language);
        _isInitialized = true;
    }

    private void SpeechToTextOnStateChanged(object? sender, SpeechToTextStateChangedEventArgs e)
    {
        State = e.State;
    }

    private void _speechToText_RecognitionResultUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
    {
        RecognitionResult += e.RecognitionResult;
    }

    private async Task ExecuteRecognitionResult()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(RecognitionResult))
            {
                Logs.Add($"Recognition completed successfully: {RecognitionResult}");
                Logs.Add(await executor.Execute(RecognitionResult));
            }
        }
        catch (Exception ex)
        {
            Logs.Add($"Error during command execution: {ex.Message}");
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task StartListen()
    {
        RecognitionResult = null;
        await SpeechToText.StartListenAsync(new SpeechToTextOptions { Culture = _speechCulture, ShouldReportPartialResults = true });
        Subscribe();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task StopListen()
    {
        await StopListening();

        if (_autoExecuteCommands)
        {
            await ExecuteRecognitionResult();
        }
    }

    private async Task StopListening()
    {
        await SpeechToText.StopListenAsync();
        Unsubscribe();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SwitchSpeechToText()
    {
        await StopListening();

        IsOfflineSpeechToText = !IsOfflineSpeechToText;
        await PersistSpeechModeAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (_isSubscribed)
        {
            return;
        }

        SpeechToText.StateChanged += SpeechToTextOnStateChanged;
        SpeechToText.RecognitionResultUpdated += _speechToText_RecognitionResultUpdated;
        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        SpeechToText.StateChanged -= SpeechToTextOnStateChanged;
        SpeechToText.RecognitionResultUpdated -= _speechToText_RecognitionResultUpdated;
        _isSubscribed = false;
    }

    private async Task PersistSpeechModeAsync()
    {
        try
        {
            var settings = await _settingsService.LoadSettingsAsync();
            settings.Speech.UseOfflineRecognition = IsOfflineSpeechToText;
            await _settingsService.SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            Logs.Add($"Failed to persist speech mode: {ex.Message}");
        }
    }

    private static CultureInfo ResolveCulture(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return CultureInfo.CurrentCulture;
        }

        try
        {
            return CultureInfo.GetCultureInfo(language);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.CurrentCulture;
        }
    }
}