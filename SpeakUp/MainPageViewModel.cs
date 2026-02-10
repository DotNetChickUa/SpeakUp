using System.Globalization;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Executor;

namespace SpeakUp;

public sealed partial class MainPageViewModel(IExecutor executor) : ObservableObject, IDisposable
{
    private ISpeechToText _speechToText = SpeechToText.Default;
    private bool _isSubscribed;

    [ObservableProperty]
    public partial string? State { get; set; }

    [ObservableProperty]
    public partial string? RecognitionResult { get; set; }

    public bool IsOfflineSpeechToText => _speechToText is OfflineSpeechToTextImplementation;


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
        if (e.RecognitionResult.IsSuccessful)
        {
            RecognitionResult = e.RecognitionResult.Text;
            RecognitionResult = await executor.Execute(RecognitionResult);
        }
        else
        {
            RecognitionResult = e.RecognitionResult.Exception.Message;
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task StartListen()
    {
        Subscribe();
        RecognitionResult = null;
        await _speechToText.StartListenAsync(new SpeechToTextOptions() { Culture = CultureInfo.CurrentCulture, ShouldReportPartialResults = true });
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task StopListen()
    {
        await _speechToText.StopListenAsync();
        Unsubscribe();
        if (!string.IsNullOrEmpty(RecognitionResult))
        {
            RecognitionResult = await executor.Execute(RecognitionResult);
        }
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SwitchSpeechToText()
    {
        await StopListen();
        Unsubscribe();

        _speechToText = _speechToText == SpeechToText.Default ? OfflineSpeechToText.Default : SpeechToText.Default;

        Subscribe();

        OnPropertyChanged(nameof(IsOfflineSpeechToText));
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

        _speechToText.StateChanged += SpeechToTextOnStateChanged;
        _speechToText.RecognitionResultUpdated += _speechToText_RecognitionResultUpdated;
        _speechToText.RecognitionResultCompleted += _speechToText_RecognitionResultCompleted;
        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed)
        {
            return;
        }

        _speechToText.StateChanged -= SpeechToTextOnStateChanged;
        _speechToText.RecognitionResultUpdated -= _speechToText_RecognitionResultUpdated;
        _speechToText.RecognitionResultCompleted -= _speechToText_RecognitionResultCompleted;
        _isSubscribed = false;
    }
}