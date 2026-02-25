using System.ComponentModel;
using Shared;

namespace Base.Extensions.LogsExtensions;

[SpeakUpTool]
public class FileLogMacroCommand
{
	[Description("Logs a message to a file.")]
	public static async Task ExecuteLogAsync(string path, string? message,
		CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrEmpty(path);
		await File.AppendAllTextAsync(path, message, cancellationToken);
	}
}