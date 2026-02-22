using System.ComponentModel;
using System.Diagnostics;
using Shared;

namespace ProcessPlugins;

[SpeakUpTool]
public static class ProcessTools
{
    [Description("Starts a new process with specified appName")]
    public static async Task<int> RunApp(string appName)
    {
        Debug.WriteLine($"{DateTime.UtcNow}: Starting process: {appName}");
        var process = Process.Start(appName);
        await Task.Delay(1000);
        return process.Id;
    }

    [Description("Kills the process with specified processId")]
    public static void CloseApp(int processId)
    {
        Debug.WriteLine($"{DateTime.UtcNow}: Killing process with ID: {processId}");
        var process = Process.GetProcessById(processId);
        process.Kill();
    }

    [Description("Get array of the processes")]
    public static string[] GetProcesses()
    {
        Debug.WriteLine($"{DateTime.UtcNow}: Getting list of processes");
        return Process.GetProcesses().Select(x => x.ProcessName).ToArray();
    }
}