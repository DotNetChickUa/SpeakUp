using System.ComponentModel;
using Shared;

namespace MailgunExtensions.MacroCommands;

[SpeakUpTool]
public class SendEmail
{
	[Description("Sends an email using the Mailgun API.")]
	public static async Task SendEmailAsync(Uri requestUri, string apiKey, string to, string from, string subject, string message)
	{
		using var mailgunService = new MailgunService();
		await mailgunService.SendEmailAsync(apiKey, requestUri, to, from, subject, message);
	}
}