using Microsoft.Extensions.AI;

namespace SpeakUp;

sealed class PluginLoadResult
{
    private readonly List<AITool> tools;
    private readonly List<PluginLoadContext> loadContexts;

    public PluginLoadResult(List<AITool> tools, List<PluginLoadContext> loadContexts)
    {
        this.tools = tools;
        this.loadContexts = loadContexts;
    }

    public IList<AITool> Tools => tools;

    public void Unload()
    {
        tools.Clear();
        foreach (var context in loadContexts)
        {
            context.Unload();
        }

        loadContexts.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}