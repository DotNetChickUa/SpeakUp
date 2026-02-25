using System.ComponentModel;
using Shared;

namespace SshExtensions.Sftp;

[SpeakUpTool]
public class UploadFile : SftpMacroCommand
{
    [Description("Uploads a file to a remote server using Sftp.")]
    public static async Task ExecuteAsync(string host, int port, string username, string password, string uploadPath, Stream stream, CancellationToken cancellationToken)
    {
        using var client = await GetClient(host, port, username, password, cancellationToken);
        client.UploadFile(stream, uploadPath);
        client.Disconnect();
    }
}