using System.ComponentModel;
using Shared;

namespace FileSystemExtensions;

[SpeakUpTool]
public class ListLocalFilesMacroCommand
{
	[Description("Lists all files in the specified directory and its subdirectories.")]
	public static async Task<IEnumerable<string>> ListFiles(string initialDirectory, CancellationToken cancellationToken)
	{
		if (Directory.Exists(initialDirectory))
		{
			var files = Directory.GetFiles(initialDirectory, "*.*", SearchOption.AllDirectories);
			return files;
		}

		return Enumerable.Empty<string>();
	}
}