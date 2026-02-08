using System.ComponentModel;
using System.Diagnostics;

namespace SpeakUp.Tools;

public static class ProcessTools
{
    [Description("Starts a new process with specified appName")]
    public static async Task<int> RunApp(string appName)
    {
        var process = Process.Start(appName);
        await Task.Delay(1000);
        return process.Id;
    }

    [Description("Kills the process with specified processId")]
    public static void CloseApp(int processId)
    {
        var process = Process.GetProcessById(processId);
        process.Kill();
    }

    [Description("Get array of the processes")]
    public static string[] GetProcesses()
    {
        return Process.GetProcesses().Select(x => x.ProcessName).ToArray();
    }
}