using System.ComponentModel;
using Shared;

namespace MegaExtensions;

using CG.Web.MegaApiClient;

[SpeakUpTool]
public class MegaListFilesMacroCommand
{
	[Description("List files from a Mega account")]
	public static async Task<IEnumerable<INode>?> ListFiles(string login, string password,
        CancellationToken cancellationToken)
	{
		var mega = new MegaCloudDrive(login, password);
		var files = await mega.ListFiles(cancellationToken);
		return files;
	}
}