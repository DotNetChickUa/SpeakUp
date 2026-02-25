using System.ComponentModel;
using Shared;

namespace Base.Extensions.LogsExtensions;

[SpeakUpTool]
public class ConsoleLogMacroCommand
{
	[Description("Logs a message to the console.")]
	public static Task ExecuteLogAsync(string? message,
		CancellationToken cancellationToken)
	{
		Console.WriteLine(message);
		return Task.CompletedTask;
	}
}