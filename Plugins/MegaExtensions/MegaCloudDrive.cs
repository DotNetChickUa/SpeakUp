namespace MegaExtensions;

using CG.Web.MegaApiClient;

public class MegaCloudDrive(string? login, string? password)
{
	private readonly IMegaApiClient client = new MegaApiClient();

	public Task<Stream> DownloadFile(string? id, CancellationToken cancellationToken)
	{
		return DoAction(async () =>
		{
			var nodes = await client.GetNodesAsync();
			var stream = await client.DownloadAsync(nodes.Single(x => x.Id == id), null, cancellationToken);
			await client.LogoutAsync();
			return stream;
		});
	}

	public Task<string> UploadFile(string rootFolderName, string filePath, Stream fileStream)
	{
		var fileName = Path.GetFileName(filePath);
		var folderName = Path.GetDirectoryName(filePath);
		return DoAction(async () =>
		{
			var nodes = await client.GetNodesAsync();

			var root = nodes.Single(x => x.Name == rootFolderName);
			var myFolder = await client.CreateFolderAsync(folderName, root);

			var node = await client.UploadAsync(fileStream, fileName, myFolder);
			return node.Id;
		});
	}

	private async Task<T> DoAction<T>(Func<Task<T>> func)
	{
		if (!client.IsLoggedIn)
		{
			await client.LoginAsync(login, password);
		}

		var result = await func();
		return result;
	}

	public Task<IEnumerable<INode>> ListFiles(CancellationToken cancellationToken)
	{
		return DoAction(async () =>
		{
			var nodes = await client.GetNodesAsync();
			await client.LogoutAsync();
			return nodes;
		});
	}
}