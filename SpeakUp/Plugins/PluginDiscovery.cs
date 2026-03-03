using System.Reflection;
using System.Runtime.Loader;

namespace SpeakUp.Plugins;

internal static class PluginDiscovery
{
    private const string PluginsDirectoryName = "Plugins";

    public static IEnumerable<string> GetManagedPluginFiles()
    {
        var pluginsPath = GetPluginsPath();
        if (!Directory.Exists(pluginsPath))
        {
            return [];
        }

        return Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories)
            .Where(IsManagedAssembly);
    }

    public static string GetPluginFileName(string pluginFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginFilePath);
        return Path.GetFileNameWithoutExtension(pluginFilePath);
    }

    public static string GetDisplayName(string pluginFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginFileName);
        return pluginFileName.Replace("Extensions", string.Empty).Replace("Macro", string.Empty);
    }

    /// <summary>
    /// Loads a plugin in a collectible context and returns assembly metadata used by plugin services.
    /// </summary>
    public static PluginMetadata InspectPlugin(string pluginFilePath, string contextPrefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(contextPrefix);

        var fileName = GetPluginFileName(pluginFilePath);
        var assemblyName = AssemblyName.GetAssemblyName(pluginFilePath);
        var loadContext = new AssemblyLoadContext($"{contextPrefix}_{fileName}", isCollectible: true);
        Assembly? assembly = null;

        try
        {
            assembly = loadContext.LoadFromAssemblyPath(pluginFilePath);
            var commandCount = assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Count(m => m.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>() is not null);

            return new PluginMetadata(assemblyName, commandCount);
        }
        finally
        {
            if (assembly is not null)
            {
                loadContext.Unload();
            }
        }
    }

    private static string GetPluginsPath() => Path.Combine(AppContext.BaseDirectory, PluginsDirectoryName);

    private static bool IsManagedAssembly(string path)
    {
        try
        {
            _ = AssemblyName.GetAssemblyName(path);
            return true;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        catch (FileLoadException)
        {
            return false;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
    }
}

internal readonly record struct PluginMetadata(AssemblyName AssemblyName, int CommandCount);
