using System.ComponentModel;
using Shared;

namespace TelegramExtensions;

[SpeakUpTool]
public class LoginMacroCommand
{
	[Description("Logs in to Telegram and returns the user ID.")]
	public static async Task<long> Login(CancellationToken cancellationToken)
	{
		var telegramService = new TelegramService();
		var user = await telegramService.Login();
		return user.ID;
	}
}