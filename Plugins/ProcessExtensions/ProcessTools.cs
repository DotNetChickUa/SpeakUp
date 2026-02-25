using System.ComponentModel;
using System.Diagnostics;
using Shared;

namespace ProcessExtensions;

[SpeakUpTool]
public static class ProcessTools
{
    [Description("Starts a new process with specified appName")]
    public static async Task<int> RunApp(string appName, string arguments = "")
    {
        Debug.WriteLine($"{DateTime.UtcNow}: Starting process: {appName}, Arguments: {arguments}");
        var processService = new ProcessService();
        var processId = await processService.Start(appName, arguments);
        return processId;
    }

    [Description("Kills the process with specified process name")]
    public static void CloseApp(string processName)
    {
        Debug.WriteLine($"{DateTime.UtcNow}: Killing process with name: {processName}");
        var processService = new ProcessService();
        processService.Kill(processName);
    }

    [Description("Get array of the processes")]
    public static string[] GetProcesses()
    {
        Debug.WriteLine($"{DateTime.UtcNow}: Getting list of processes");
        var processService = new ProcessService();
        return processService.List().ToArray();
    }
}