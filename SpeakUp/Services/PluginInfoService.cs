using SpeakUp.Models;
using System.Reflection;
using System.Runtime.Loader;

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
        var pluginsPath = Path.Combine(AppContext.BaseDirectory, "Plugins");

        if (!Directory.Exists(pluginsPath))
        {
            _cachedStatuses = statuses;
            return statuses;
        }

        var pluginFiles = Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories);

        foreach (var pluginFile in pluginFiles)
        {
            try
            {
                if (!IsManagedAssembly(pluginFile))
                {
                    continue;
                }

                var fileName = Path.GetFileNameWithoutExtension(pluginFile);
                var assemblyName = AssemblyName.GetAssemblyName(pluginFile);
                
                var loadContext = new AssemblyLoadContext($"PluginInfo_{fileName}", isCollectible: true);
                Assembly? assembly = null;
                
                try
                {
                    assembly = loadContext.LoadFromAssemblyPath(pluginFile);
                    
                    var commandCount = assembly.GetTypes()
                        .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                        .Count(m => m.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>() != null);

                    var category = DetermineCategory(fileName);

                    statuses.Add(new PluginStatus
                    {
                        Name = fileName.Replace("Extensions", "").Replace("Macro", ""),
                        Category = category,
                        IsLoaded = true,
                        CommandCount = commandCount,
                        Version = assemblyName.Version?.ToString() ?? "1.0.0",
                        Description = $"{commandCount} commands available"
                    });
                }
                finally
                {
                    if (assembly != null)
                    {
                        loadContext.Unload();
                    }
                }
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

    private static bool IsManagedAssembly(string path)
    {
        try
        {
            _ = AssemblyName.GetAssemblyName(path);
            return true;
        }
        catch
        {
            return false;
        }
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
