using System.ComponentModel;
using Shared;

namespace SendGridExtensions.MacroCommands;

[SpeakUpTool]
public class SendEmailMacroCommand
{
    [Description("Sends an email using SendGrid with the specified parameters.")]
    public static async Task SendOutEmail(string apiKey, string fromEmail, string toEmail, string subject, string plainTextContent, string htmlContent, CancellationToken cancellationToken)
    {
        var sendGridService = new SendGridService();

        await sendGridService.SendEmailAsync(apiKey, fromEmail, string.Empty, toEmail, string.Empty, subject, plainTextContent,
                                                     htmlContent);
    }
}