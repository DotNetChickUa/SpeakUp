using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Models;
using SpeakUp.Services;
using System.Collections.ObjectModel;

namespace SpeakUp.Pages;

public sealed partial class CommandHistoryPageViewModel(ICommandHistoryService historyService) : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<CommandHistoryEntry> _commands = [];

    [ObservableProperty]
    private ObservableCollection<SessionInfo> _sessions = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private SessionInfo? _selectedSession;

    [ObservableProperty]
    private int _totalCommands;

    [ObservableProperty]
    private int _successCount;

    [ObservableProperty]
    private int _errorCount;

    public async Task InitializeAsync()
    {
        await LoadRecentCommandsAsync();
        await LoadSessionsAsync();
    }

    [RelayCommand]
    private async Task LoadRecentCommandsAsync()
    {
        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var commands = await historyService.GetRecentCommandsAsync(100);
            
            Commands.Clear();
            foreach (var cmd in commands)
            {
                Commands.Add(cmd);
            }

            UpdateStatistics(commands);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to load commands: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadSessionsAsync()
    {
        try
        {
            var sessions = await historyService.GetAllSessionsAsync();
            
            Sessions.Clear();
            foreach (var session in sessions)
            {
                Sessions.Add(session);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to load sessions: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadRecentCommandsAsync();
            return;
        }

        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var commands = await historyService.SearchCommandsAsync(SearchText);
            
            Commands.Clear();
            foreach (var cmd in commands)
            {
                Commands.Add(cmd);
            }

            UpdateStatistics(commands);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to search commands: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadSessionCommandsAsync()
    {
        if (SelectedSession == null || IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var commands = await historyService.GetSessionCommandsAsync(SelectedSession.SessionId);
            
            Commands.Clear();
            foreach (var cmd in commands)
            {
                Commands.Add(cmd);
            }

            UpdateStatistics(commands);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to load session commands: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task DeleteOldCommandsAsync()
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            "Delete Old Commands",
            "Delete commands older than 30 days?",
            "Delete",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        try
        {
            var deleted = await historyService.DeleteOldCommandsAsync(30);
            await Shell.Current.DisplayAlertAsync("Success", $"Deleted {deleted} old commands", "OK");
            await LoadRecentCommandsAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to delete commands: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task ClearSearchAsync()
    {
        SearchText = string.Empty;
        SelectedSession = null;
        await LoadRecentCommandsAsync();
    }

    [RelayCommand]
    private async Task ExportLogsAsync()
    {
        try
        {
            var json = await historyService.ExportCommandsAsync();
            
            var fileName = $"speakup_history_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            await File.WriteAllTextAsync(filePath, json);
            
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Export Command History",
                File = new ShareFile(filePath)
            });
            
            await Shell.Current.DisplayAlertAsync("Success", "Command history exported successfully", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlertAsync("Error", $"Failed to export logs: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private static async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void UpdateStatistics(List<CommandHistoryEntry> commands)
    {
        TotalCommands = commands.Count;
        SuccessCount = commands.Count(c => c.Status == ExecutionStatus.Success);
        ErrorCount = commands.Count(c => c.Status == ExecutionStatus.Error);
    }

    partial void OnSelectedSessionChanged(SessionInfo? value)
    {
        if (value != null)
        {
            _ = LoadSessionCommandsAsync();
        }
    }
}
