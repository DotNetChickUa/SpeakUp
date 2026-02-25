using System.ComponentModel;
using Shared;

namespace MegaExtensions;

[SpeakUpTool]
public class MegaDownloadFileMacroCommand
{
	[Description("Downloads a file from Mega using the provided login credentials and file ID.")]
	public static async Task<Stream> DownloadFile(string login, string password, string fileId, CancellationToken cancellationToken)
	{
		var mega = new MegaCloudDrive(login, password);
		var stream = await mega.DownloadFile(fileId, cancellationToken);
		return stream;
	}
}