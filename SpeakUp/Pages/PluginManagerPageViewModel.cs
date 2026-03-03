using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Models;
using SpeakUp.Services;
using System.Collections.ObjectModel;

namespace SpeakUp.Pages;

public sealed partial class PluginManagerPageViewModel : ObservableObject
{
    private readonly IPluginManagerService _pluginManager;
    private readonly IErrorHandlingService _errorHandler;

    public ObservableCollection<PluginInfo> Plugins { get; } = [];
    public ObservableCollection<PluginInfo> FilteredPlugins { get; } = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "All";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _totalPlugins;

    [ObservableProperty]
    private int _enabledPlugins;

    [ObservableProperty]
    private int _totalCommands;

    public ObservableCollection<string> Categories { get; } = new()
    {
        "All",
        "Communication",
        "Email/SMS",
        "Cloud Storage",
        "System",
        "Data",
        "Utilities",
        "Other"
    };

    public PluginManagerPageViewModel(IPluginManagerService pluginManager, IErrorHandlingService errorHandler)
    {
        _pluginManager = pluginManager;
        _errorHandler = errorHandler;
    }

    public async Task InitializeAsync()
    {
        await LoadPluginsAsync();
    }

    [RelayCommand]
    private async Task LoadPluginsAsync()
    {
        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            
            var plugins = await _pluginManager.GetAllPluginsAsync();
            
            Plugins.Clear();
            foreach (var plugin in plugins)
            {
                Plugins.Add(plugin);
            }

            ApplyFilters();
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(
                ex,
                "Loading plugins",
                "Check if the Plugins folder exists and contains valid plugin DLLs");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task TogglePluginAsync(PluginInfo plugin)
    {
        if (plugin == null)
        {
            return;
        }

        try
        {
            var success = plugin.IsEnabled
                ? await _pluginManager.DisablePluginAsync(plugin.Name)
                : await _pluginManager.EnablePluginAsync(plugin.Name);

            if (success)
            {
                plugin.IsEnabled = !plugin.IsEnabled;
                UpdateStatistics();
                
                await Shell.Current.DisplayAlertAsync(
                    "Success",
                    $"Plugin '{plugin.Name}' has been {(plugin.IsEnabled ? "enabled" : "disabled")}.\nRestart the app to apply changes.",
                    "OK");
            }
            else
            {
                await _errorHandler.ShowErrorAsync(
                    "Plugin Toggle Failed",
                    $"Could not toggle plugin '{plugin.Name}'");
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, $"Toggling plugin '{plugin.Name}'");
        }
    }

    [RelayCommand]
    private async Task EnableAllPluginsAsync()
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            "Enable All Plugins",
            "Enable all available plugins? This may increase startup time and memory usage.",
            "Enable All",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        try
        {
            foreach (var plugin in Plugins.Where(p => !p.IsEnabled))
            {
                await _pluginManager.EnablePluginAsync(plugin.Name);
                plugin.IsEnabled = true;
            }

            UpdateStatistics();
            
            await Shell.Current.DisplayAlertAsync(
                "Success",
                "All plugins have been enabled. Restart the app to apply changes.",
                "OK");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Enabling all plugins");
        }
    }

    [RelayCommand]
    private async Task DisableAllPluginsAsync()
    {
        var confirm = await _errorHandler.ConfirmDestructiveActionAsync(
            "disable all plugins",
            "All plugin functionality will be unavailable until re-enabled.");

        if (!confirm)
        {
            return;
        }

        try
        {
            foreach (var plugin in Plugins.Where(p => p.IsEnabled))
            {
                await _pluginManager.DisablePluginAsync(plugin.Name);
                plugin.IsEnabled = false;
            }

            UpdateStatistics();
            
            await Shell.Current.DisplayAlertAsync(
                "Success",
                "All plugins have been disabled. Restart the app to apply changes.",
                "OK");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Disabling all plugins");
        }
    }

    [RelayCommand]
    private void ApplyFilters()
    {
        var filtered = Plugins.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLowerInvariant();
            filtered = filtered.Where(p =>
                p.Name.Contains(searchLower, StringComparison.OrdinalIgnoreCase) ||
                p.Description?.Contains(searchLower, StringComparison.OrdinalIgnoreCase) == true ||
                p.Category.Contains(searchLower, StringComparison.OrdinalIgnoreCase));
        }

        // Apply category filter
        if (SelectedCategory != "All")
        {
            filtered = filtered.Where(p => p.Category == SelectedCategory);
        }

        FilteredPlugins.Clear();
        foreach (var plugin in filtered)
        {
            FilteredPlugins.Add(plugin);
        }
    }

    [RelayCommand]
    private async Task RefreshPluginsAsync()
    {
        await _pluginManager.ReloadPluginsAsync();
        await LoadPluginsAsync();
    }

    [RelayCommand]
    private static async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void UpdateStatistics()
    {
        TotalPlugins = Plugins.Count;
        EnabledPlugins = Plugins.Count(p => p.IsEnabled);
        TotalCommands = Plugins.Sum(p => p.CommandCount);
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilters();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        ApplyFilters();
    }
}
