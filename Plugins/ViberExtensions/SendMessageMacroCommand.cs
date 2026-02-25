using System.ComponentModel;
using Shared;

namespace ViberExtensions;

[SpeakUpTool]
public class SendMessageMacroCommand
{
	[Description("Sends a message via Viber.")]
	public static async Task<string> Post(string accessToken, string chatId, string text, CancellationToken cancellationToken)
	{
		var messageId = await ViberService.NewMessage(accessToken, chatId, text, cancellationToken);
		return messageId;
	}
}