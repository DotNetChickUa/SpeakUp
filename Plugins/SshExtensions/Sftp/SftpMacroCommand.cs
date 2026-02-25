using Renci.SshNet;

namespace SshExtensions.Sftp;

public abstract class SftpMacroCommand
{
    protected static async Task<SftpClient> GetClient(string host, int port, string username, string password, CancellationToken cancellationToken)
	{
		var client = new SftpClient(host, port, username, password);
		await client.ConnectAsync(cancellationToken);
		return client;
    }
}