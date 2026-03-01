using SpeakUp.Models;

namespace SpeakUp.Services;

/// <summary>
/// Service for centralized error handling and display
/// </summary>
public interface IErrorHandlingService
{
    /// <summary>
    /// Handle and display an error
    /// </summary>
    Task HandleErrorAsync(ErrorInfo error);

    /// <summary>
    /// Handle an exception
    /// </summary>
    Task HandleExceptionAsync(Exception exception, string context, string? suggestedAction = null);

    /// <summary>
    /// Show a user-friendly error message
    /// </summary>
    Task ShowErrorAsync(string title, string message, ErrorSeverity severity = ErrorSeverity.Error);

    /// <summary>
    /// Show a confirmation dialog for potentially destructive actions
    /// </summary>
    Task<bool> ConfirmDestructiveActionAsync(string action, string details);
}

/// <summary>
/// Implementation of error handling service
/// </summary>
internal sealed class ErrorHandlingService : IErrorHandlingService
{
    public async Task HandleErrorAsync(ErrorInfo error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var message = error.DisplayMessage;
        
        if (error.Exception != null)
        {
            message += $"\n\nDetails: {error.Exception.Message}";
        }

        if (!string.IsNullOrWhiteSpace(error.SuggestedAction))
        {
            message += $"\n\n💡 Suggestion: {error.SuggestedAction}";
        }

        var title = $"{error.Icon} {error.Title}";

        await Application.Current!.Dispatcher.DispatchAsync(async () =>
        {
            await Shell.Current.DisplayAlert(title, message, "OK");
        });
    }

    public async Task HandleExceptionAsync(Exception exception, string context, string? suggestedAction = null)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var error = new ErrorInfo
        {
            Title = "An Error Occurred",
            Message = GetUserFriendlyMessage(exception),
            Context = context,
            Exception = exception,
            SuggestedAction = suggestedAction ?? GetSuggestedAction(exception),
            Severity = GetSeverity(exception)
        };

        await HandleErrorAsync(error);
    }

    public async Task ShowErrorAsync(string title, string message, ErrorSeverity severity = ErrorSeverity.Error)
    {
        var error = new ErrorInfo
        {
            Title = title,
            Message = message,
            Severity = severity
        };

        await HandleErrorAsync(error);
    }

    public async Task<bool> ConfirmDestructiveActionAsync(string action, string details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(action);

        var message = $"This action will {action}.\n\n{details}\n\nThis cannot be undone. Continue?";
        
        return await Application.Current!.Dispatcher.DispatchAsync(async () =>
        {
            return await Shell.Current.DisplayAlert(
                "⚠️ Confirm Action",
                message,
                "Continue",
                "Cancel");
        });
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            InvalidOperationException => "The operation could not be completed. Please check your settings and try again.",
            UnauthorizedAccessException => "Access denied. Please check your permissions.",
            System.Net.Http.HttpRequestException => "Network error. Please check your internet connection.",
            TimeoutException => "The operation timed out. Please try again.",
            ArgumentException => "Invalid input provided. Please check your data and try again.",
            FileNotFoundException => "A required file could not be found.",
            _ => exception.Message
        };
    }

    private static string? GetSuggestedAction(Exception exception)
    {
        return exception switch
        {
            InvalidOperationException => "Verify your AI API key is configured in Settings",
            UnauthorizedAccessException => "Check app permissions in system settings",
            System.Net.Http.HttpRequestException => "Check your internet connection and try again",
            TimeoutException => "Try again or check your network connection",
            FileNotFoundException => "Ensure all required plugins and files are present",
            _ => null
        };
    }

    private static ErrorSeverity GetSeverity(Exception exception)
    {
        return exception switch
        {
            ArgumentException or ArgumentNullException => ErrorSeverity.Warning,
            UnauthorizedAccessException or InvalidOperationException => ErrorSeverity.Error,
            OutOfMemoryException or StackOverflowException => ErrorSeverity.Critical,
            _ => ErrorSeverity.Error
        };
    }
}
