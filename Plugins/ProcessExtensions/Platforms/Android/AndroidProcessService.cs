namespace ProcessExtensions;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;

internal class ProcessService
{
	public Task<int> Start(string packageName, string? arguments)
	{
		var intent = Application.Context.PackageManager?.GetLaunchIntentForPackage(packageName);
		if (intent == null)
		{
			intent = new Intent(Intent.ActionView);
			intent.SetData(Uri.Parse("market://details?id=" + packageName));
		}

		intent.AddFlags(ActivityFlags.NewTask);

		try
		{
			Application.Context.StartActivity(intent);
		}
		catch (ActivityNotFoundException)
		{
			return Task.FromResult(0);
		}

		return Task.FromResult(GetAppPid(packageName));
	}

	public IEnumerable<string> List()
	{
		return RunningAppProcessInfos().Where(info => info.ProcessName != null).Select(info => info.ProcessName!);
	}

	public void Kill(string packageName)
	{
		var amg = Application.Context.GetSystemService(Context.ActivityService) as ActivityManager;
		var processes = RunningAppProcessInfos();
		foreach (var info in processes)
		{
			if (info.ProcessName == packageName)
			{
				// kill selected process
				Process.KillProcess(info.Pid);
				Process.SendSignal(info.Pid, Signal.Kill);
				amg?.KillBackgroundProcesses(packageName);
			}
		}
	}

	private static int GetAppPid(string processName)
	{
		var process = RunningAppProcessInfos().FirstOrDefault(info => info.ProcessName == processName);
		return process?.Pid ?? 0;
	}

	private static IEnumerable<ActivityManager.RunningAppProcessInfo> RunningAppProcessInfos()
	{
		var am = Application.Context.GetSystemService(Context.ActivityService) as ActivityManager;
		return am?.RunningAppProcesses ?? new List<ActivityManager.RunningAppProcessInfo>();
	}
}