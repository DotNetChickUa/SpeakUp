using System.ComponentModel;
using Shared;

namespace SshExtensions.Scp;

[SpeakUpTool]
public class UploadFile : ScpMacroCommand
{
    [Description("Uploads a file to a remote server using SCP.")]
    public static async Task ExecuteAsync(string host, int port, string username, string password, string uploadPath, Stream stream, CancellationToken cancellationToken)
    {
        using var client = await GetClient(host, port, username, password, cancellationToken);
        client.Upload(stream, uploadPath);
        client.Disconnect();
    }
}