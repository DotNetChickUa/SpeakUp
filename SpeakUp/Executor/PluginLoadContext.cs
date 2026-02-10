using System.Reflection;
using System.Runtime.Loader;

namespace SpeakUp;

sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver resolver;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true)
    {
        resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (AssemblyLoadContext.Default.Assemblies.Any(a => string.Equals(a.GetName().Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return null;
        }

        var assemblyPath = resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath is null ? null : LoadFromAssemblyPath(assemblyPath);
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(libraryPath);
    }
}