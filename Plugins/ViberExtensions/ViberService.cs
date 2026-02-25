namespace ViberExtensions;

using Com.CloudRail.SI;
using Com.CloudRail.SI.Services;

public static class ViberService
{
	public static async Task<string> NewMessage(string? accessToken,
		string? chatId,
		string? message,
		CancellationToken cancellationToken)
	{
		CloudRail.AppKey = GetKey();
		await Task.CompletedTask;
		var service = new Viber(null, "[Bot Token]", "[Webhook URL]", "[Bot Name]");
		var messageResult = service.SendMessage(chatId, message);
		return messageResult.GetMessageId();
	}

	private static string GetKey()
	{
		var random = new Random();
		const string chars = "0123456789abcdef";
		return new string(Enumerable.Repeat(chars, 24).Select(s => s[random.Next(s.Length)]).ToArray());
	}
}