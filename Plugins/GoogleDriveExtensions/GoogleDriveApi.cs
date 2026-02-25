namespace GoogleDriveExtensions;

using System.Diagnostics.CodeAnalysis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

internal static class GoogleDriveApi
{
	public static async Task<GoogleFilesListResult> ListFiles(string? accessToken, CancellationToken cancellationToken)
	{
		var credential = GoogleCredential.FromAccessToken(accessToken);
		using var driveService = new DriveService(new BaseClientService.Initializer()
		{
			HttpClientInitializer = credential
		});

		var request = driveService.Files.List();
		request.Fields = "nextPageToken, files(id, name, createdTime, mimeType, modifiedTime, size)";

		try
		{
			var filesList = await request.ExecuteAsync(cancellationToken);
			return new GoogleFilesListResult(filesList.Files.Select(x => new GoogleFile()
			{
				Id = x.Id,
				Name = x.Name,
				CreatedTime = x.CreatedTimeDateTimeOffset?.DateTime,
				MimeType = x.MimeType,
				ModifiedTime = x.ModifiedTimeDateTimeOffset?.DateTime,
				Size = x.Size
			}).ToList(), null);
		}
		catch (Exception e)
		{
			return new GoogleFilesListResult(null, e);
		}
	}
}


public record GoogleFilesListResult(List<GoogleFile>? Files, [property: MemberNotNull("Files")] Exception? Exception);