using System.ComponentModel;
using Shared;

namespace GoogleDriveExtensions;

[SpeakUpTool]
public class GoogleDriveListFilesMacroCommand
{
	[Description("Lists files in the user's Google Drive account. Requires an access token with appropriate permissions.")]
	public static async Task<IEnumerable<GoogleFile>?> ListFiles(string accessToken, CancellationToken cancellationToken)
	{
		var listResult = await GoogleDriveApi.ListFiles(accessToken, cancellationToken);
		if (listResult.Exception is null)
		{
			return listResult.Files;
		}

		return Enumerable.Empty<GoogleFile>();
	}
}