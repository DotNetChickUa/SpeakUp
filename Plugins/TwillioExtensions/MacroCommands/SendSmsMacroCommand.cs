using System.ComponentModel;
using Shared;

namespace TwilioExtensions.MacroCommands;

[SpeakUpTool]
public class SendSmsMacroCommand
{
	[Description("Sends a text message using Twilio's API.")]
	public static async Task SendTextMessage(string accountSid, string authToken, string toPhoneNumber, string fromPhoneNumber, string message, IFlow flow, IProgress<MacroActionResult> progress,
        CancellationToken cancellationToken)
	{
		var twilioService = new TwilioService();
        await twilioService.SendSmsAsync(accountSid, authToken, toPhoneNumber, fromPhoneNumber, message);
	}
}