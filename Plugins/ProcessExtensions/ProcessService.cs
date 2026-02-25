#if !ANDROID && !IOS && !MACCATALYST && !TIZEN && !WINDOWS
namespace ProcessExtensions;

internal class ProcessService
{
	public IEnumerable<string> List()
	{
		return Enumerable.Empty<string>();
	}

	public void Kill(string processName)
	{
		throw new NotImplementedException();
	}

	public Task<int> Start(string uri, string? arguments)
	{
		return Task.FromResult(0);
	}
}
#endif