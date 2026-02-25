using System.ComponentModel;
using Shared;

namespace TelegramExtensions;

[SpeakUpTool]
public class SendMessageMacroCommand
{
	[Description("Sends a message to a specified Telegram chat using the provided access token.")]
	public static async Task<int> Post(string text, long chatId, CancellationToken cancellationToken)
	{
		var telegramService = new TelegramService();
		var messageId = await telegramService.NewMessage(chatId, text);
		return messageId;
	}
}