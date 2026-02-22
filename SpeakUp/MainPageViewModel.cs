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
    public partial string? State { get; set; }

    [ObservableProperty]
    public partial string? RecognitionResult { get; set; }

    [ObservableProperty]
    public bool IsOfflineSpeechToText { get; set; }

    private ISpeechToText SpeechToText => IsOfflineSpeechToText ? offlineSpeechToText : onlineSpeechToText;


    private void SpeechToTextOnStateChanged(object? sender, SpeechToTextStateChangedEventArgs e)
    {
        State = e.State.ToString();
    }

    private void _speechToText_RecognitionResultUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
    {
        RecognitionResult += e.RecognitionResult;
    }

    private async void _speechToText_RecognitionResultCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
    {
        try
        {
            if (e.RecognitionResult.IsSuccessful)
            {
                Logs.Add($"Recognition completed successfully: {e.RecognitionResult.Text}");
                Logs.Add(await executor.Execute(e.RecognitionResult.Text));
            }
            else
            {
                Logs.Add($"Recognition failed: {e.RecognitionResult.Exception.Message}");
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
        await SpeechToText.StopListenAsync();
        Unsubscribe();
        if (!string.IsNullOrEmpty(RecognitionResult))
        {
            Logs.Add(await executor.Execute(RecognitionResult));
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SwitchSpeechToText()
    {
        await StopListen();

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
        SpeechToText.RecognitionResultCompleted += _speechToText_RecognitionResultCompleted;
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
        SpeechToText.RecognitionResultCompleted -= _speechToText_RecognitionResultCompleted;
        _isSubscribed = false;
    }
}