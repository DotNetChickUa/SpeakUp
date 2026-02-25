namespace ProcessExtensions;

using System.Diagnostics;
using Foundation;
using UIKit;

internal class ProcessService
{
	public Task<int> Start(string uri, string? arguments)
	{
		var dispatcher = Dispatcher.GetForCurrentThread();
		if (dispatcher is not null)
		{
			return dispatcher.DispatchAsync(async () =>
			{
				try
				{
					var canOpen = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(uri));

					if (canOpen)
					{
						await UIApplication.SharedApplication.OpenUrlAsync(
							new NSUrl(uri), new UIApplicationOpenUrlOptions());
					}
				}
				catch
				{
					// ignored
				}

				return 0;
			});
		}

		return Task.FromResult(0);
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
				process.Close();
			}
		}
		catch
		{
			// ignored
		}
	}
}