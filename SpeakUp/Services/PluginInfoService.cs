using SpeakUp.Models;
using SpeakUp.Plugins;

namespace SpeakUp.Services;

/// <summary>
/// Service for retrieving plugin information and status
/// </summary>
public interface IPluginInfoService
{
    /// <summary>
    /// Get status of all available plugins
    /// </summary>
    Task<List<PluginStatus>> GetAllPluginStatusesAsync();

    /// <summary>
    /// Get count of loaded plugins
    /// </summary>
    int GetLoadedPluginCount();

    /// <summary>
    /// Get total command count
    /// </summary>
    int GetTotalCommandCount();
}

/// <summary>
/// Implementation of plugin info service
/// </summary>
internal sealed class PluginInfoService : IPluginInfoService
{
    private List<PluginStatus>? _cachedStatuses;

    public async Task<List<PluginStatus>> GetAllPluginStatusesAsync()
    {
        if (_cachedStatuses != null)
        {
            return _cachedStatuses;
        }

        await Task.CompletedTask;

        var statuses = new List<PluginStatus>();
        var pluginFiles = PluginDiscovery.GetManagedPluginFiles();

        foreach (var pluginFile in pluginFiles)
        {
            try
            {
                var fileName = PluginDiscovery.GetPluginFileName(pluginFile);
                var metadata = PluginDiscovery.InspectPlugin(pluginFile, "PluginInfo");
                var category = DetermineCategory(fileName);

                statuses.Add(new PluginStatus
                {
                    Name = PluginDiscovery.GetDisplayName(fileName),
                    Category = category,
                    IsLoaded = true,
                    CommandCount = metadata.CommandCount,
                    Version = metadata.AssemblyName.Version?.ToString() ?? "1.0.0",
                    Description = $"{metadata.CommandCount} commands available"
                });
            }
            catch
            {
                // Ignore plugins that can't be loaded
            }
        }

        _cachedStatuses = statuses;
        return statuses;
    }

    public int GetLoadedPluginCount()
    {
        return _cachedStatuses?.Count(p => p.IsLoaded) ?? 0;
    }

    public int GetTotalCommandCount()
    {
        return _cachedStatuses?.Sum(p => p.CommandCount) ?? 0;
    }

    private static string DetermineCategory(string fileName)
    {
        return fileName.ToLowerInvariant() switch
        {
            var n when n.Contains("telegram") || n.Contains("slack") || n.Contains("viber") ||
                      n.Contains("twitter") || n.Contains("facebook") => "Communication",
            var n when n.Contains("sendgrid") || n.Contains("mailgun") || n.Contains("email") ||
                      n.Contains("twilio") || n.Contains("elastic") => "Email/SMS",
            var n when n.Contains("s3") || n.Contains("drive") || n.Contains("mega") => "Cloud Storage",
            var n when n.Contains("file") || n.Contains("process") || n.Contains("ssh") ||
                      n.Contains("device") => "System",
            var n when n.Contains("database") || n.Contains("insights") || n.Contains("http") => "Data",
            var n when n.Contains("abstract") || n.Contains("random") => "Utilities",
            _ => "Other"
        };
    }
}
