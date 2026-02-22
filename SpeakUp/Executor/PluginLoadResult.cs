using Microsoft.Extensions.AI;

namespace SpeakUp.Executor;

sealed class PluginLoadResult(List<AITool> tools, List<PluginLoadContext> loadContexts)
{
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