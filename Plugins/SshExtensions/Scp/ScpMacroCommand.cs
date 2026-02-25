namespace SshExtensions.Scp;

using Renci.SshNet;

public abstract class ScpMacroCommand
{
    protected static async Task<ScpClient> GetClient(string host, int port, string username, string password, CancellationToken cancellationToken)
	{
		var client = new ScpClient(host, port, username, password);
		client.RemotePathTransformation = RemotePathTransformation.ShellQuote;
		await client.ConnectAsync(cancellationToken);
		return client;
    }
}