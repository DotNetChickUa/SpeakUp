using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Executor;

namespace SpeakUp;

public sealed partial class MainPageViewModel(
    IExecutor executor,
    [FromKeyedServices(nameof(SpeechToTextImplementation))] ISpeechToText onlineSpeechToText,
    [FromKeyedServices(nameof(OfflineSpeechToTextImplementation))] ISpeechToText offlineSpeechToText) : ObservableObject, IDisposable
{
    private bool _isSubscribed;

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
        await SpeechToText.StartListenAsync(new SpeechToTextOptions() { Culture = CultureInfo.CurrentCulture, ShouldReportPartialResults = true });
        Subscribe();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task StopListen()
    {
        await StopListening();
        await ExecuteRecognitionResult();
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
}