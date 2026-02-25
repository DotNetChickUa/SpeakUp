using System.ComponentModel;
using Shared;

namespace SlackExtensions;

[SpeakUpTool]
public class SendMessageMacroCommand
{
	[Description("Sends a message to a specified Slack channel using the provided access token.")]
	public static async Task<string> Post(string accessToken, string channel, string message, CancellationToken cancellationToken)
	{
		var messageId = await SlackService.NewMessage(accessToken, channel, message, cancellationToken);
		return messageId;
	}
}