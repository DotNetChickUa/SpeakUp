using System.ComponentModel;
using Shared;

namespace SshExtensions.Sftp;

[SpeakUpTool]
public class DownloadFile : SftpMacroCommand
{
    [Description("Downloads a file from a remote server using Sftp.")]
    public static async Task ExecuteAsync(string host, int port, string username, string password, string remoteFilePath, Stream stream, CancellationToken cancellationToken)
    {
        using var client = await GetClient(host, port, username, password, cancellationToken);
        client.DownloadFile(remoteFilePath, stream);
        client.Disconnect();
    }
}