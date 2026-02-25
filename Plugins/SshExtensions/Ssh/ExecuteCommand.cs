using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Shared;

namespace SshExtensions.Ssh;

[SpeakUpTool]

public class ExecuteCommand : SshMacroCommand
{
    [Description("Executes a command on a remote server using SSH.")]
    public static async Task ExecuteAsync(string host, int port, string username, string password, string command, CancellationToken cancellationToken)
    {
        using var client = await GetClient(host, port, username, password, cancellationToken);
        client.RunCommand(command);
        client.Disconnect();
    }
}