using SpeakUp.Models;
using SpeakUp.Plugins;
using System.Collections.ObjectModel;

namespace SpeakUp.Services;

/// <summary>
/// Service for managing plugin loading, unloading, and configuration
/// </summary>
public interface IPluginManagerService
{
    /// <summary>
    /// Get all available plugins with their status
    /// </summary>
    Task<ObservableCollection<PluginInfo>> GetAllPluginsAsync();

    /// <summary>
    /// Enable a plugin
    /// </summary>
    Task<bool> EnablePluginAsync(string pluginName);

    /// <summary>
    /// Disable a plugin
    /// </summary>
    Task<bool> DisablePluginAsync(string pluginName);

    /// <summary>
    /// Check if a plugin is enabled
    /// </summary>
    Task<bool> IsPluginEnabledAsync(string pluginName);

    /// <summary>
    /// Reload all plugins
    /// </summary>
    Task ReloadPluginsAsync();

    /// <summary>
    /// Get enabled plugins count
    /// </summary>
    int GetEnabledPluginCount();
}

/// <summary>
/// Implementation of plugin manager service
/// </summary>
internal sealed class PluginManagerService : IPluginManagerService
{
    private readonly ISettingsService _settingsService;
    private readonly IPluginInfoService _pluginInfoService;
    private ObservableCollection<PluginInfo>? _cachedPlugins;

    public PluginManagerService(ISettingsService settingsService, IPluginInfoService pluginInfoService)
    {
        _settingsService = settingsService;
        _pluginInfoService = pluginInfoService;
    }

    public async Task<ObservableCollection<PluginInfo>> GetAllPluginsAsync()
    {
        if (_cachedPlugins != null)
        {
            return _cachedPlugins;
        }

        var settings = await _settingsService.LoadSettingsAsync();
        var pluginStatuses = await _pluginInfoService.GetAllPluginStatusesAsync();

        var plugins = new ObservableCollection<PluginInfo>();
        var pluginFiles = PluginDiscovery.GetManagedPluginFiles();

        foreach (var file in pluginFiles)
        {
            try
            {
                var fileName = PluginDiscovery.GetPluginFileName(file);
                var displayName = PluginDiscovery.GetDisplayName(fileName);
                var fileInfo = new FileInfo(file);

                var status = pluginStatuses.FirstOrDefault(p => p.Name.Contains(displayName, StringComparison.Ordinal));

                var pluginInfo = new PluginInfo
                {
                    Name = displayName,
                    Category = status?.Category ?? "Other",
                    IsLoaded = status?.IsLoaded ?? false,
                    CommandCount = status?.CommandCount ?? 0,
                    Version = status?.Version ?? "1.0.0",
                    Description = status?.Description ?? $"{fileName} plugin",
                    FilePath = file,
                    FileSize = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime,
                    IsEnabled = settings.Plugins.AutoLoadPlugins ||
                                settings.Plugins.EnabledPlugins.Contains(fileName) ||
                                settings.Plugins.EnabledPlugins.Contains(displayName)
                };

                plugins.Add(pluginInfo);
            }
            catch
            {
                // Skip plugins that can't be loaded
            }
        }

        _cachedPlugins = plugins;
        return plugins;
    }

    public async Task<bool> EnablePluginAsync(string pluginName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginName);

        try
        {
            var settings = await _settingsService.LoadSettingsAsync();

            if (!settings.Plugins.EnabledPlugins.Contains(pluginName))
            {
                settings.Plugins.EnabledPlugins.Add(pluginName);
                await _settingsService.SaveSettingsAsync(settings);
            }

            if (_cachedPlugins != null)
            {
                var plugin = _cachedPlugins.FirstOrDefault(p => p.Name == pluginName || p.FilePath.Contains(pluginName, StringComparison.Ordinal));
                if (plugin != null)
                {
                    plugin.IsEnabled = true;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DisablePluginAsync(string pluginName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginName);

        try
        {
            var settings = await _settingsService.LoadSettingsAsync();
            settings.Plugins.EnabledPlugins.Remove(pluginName);
            await _settingsService.SaveSettingsAsync(settings);

            if (_cachedPlugins != null)
            {
                var plugin = _cachedPlugins.FirstOrDefault(p => p.Name == pluginName || p.FilePath.Contains(pluginName, StringComparison.Ordinal));
                if (plugin != null)
                {
                    plugin.IsEnabled = false;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> IsPluginEnabledAsync(string pluginName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginName);

        var normalizedName = PluginDiscovery.GetDisplayName(pluginName);
        var settings = await _settingsService.LoadSettingsAsync();
        return settings.Plugins.AutoLoadPlugins ||
               settings.Plugins.EnabledPlugins.Contains(pluginName) ||
               settings.Plugins.EnabledPlugins.Contains(normalizedName);
    }

    public async Task ReloadPluginsAsync()
    {
        _cachedPlugins = null;
        await GetAllPluginsAsync();
    }

    public int GetEnabledPluginCount()
    {
        return _cachedPlugins?.Count(p => p.IsEnabled) ?? 0;
    }
}
