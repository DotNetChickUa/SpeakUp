namespace ProcessExtensions;

using System.Diagnostics;

internal class ProcessService
{
	public Task<int> Start(string name, string? arguments)
	{
		var process = Process.Start(name, arguments ?? string.Empty);
		return Task.FromResult(process.Id);
	}

	public IEnumerable<string> List()
	{
		return Process.GetProcesses().Select(p => p.ProcessName);
	}

	public void Kill(string processName)
	{
		try
		{
			foreach (var process in Process.GetProcessesByName(processName))
			{
				process.Kill(true);
			}
		}
		catch
		{
			// ignored
		}
	}
}