namespace SshExtensions.Ssh;

using Renci.SshNet;

public abstract class SshMacroCommand
{
    protected static async Task<SshClient> GetClient(string host, int port, string username, string password, CancellationToken cancellationToken)
    {
        var client = new SshClient(host, port, username, password);
        await client.ConnectAsync(cancellationToken);
        return client;
    }
}