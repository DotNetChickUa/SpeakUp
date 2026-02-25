namespace SendGridExtensions;

using SendGrid;
using SendGrid.Helpers.Mail;

public class SendGridService : ISendGridService
{
	public Task SendEmailAsync(string? apiKey,
		string? fromEmail,
		string? fromName,
		string? toEmail,
		string? toName,
		string? subject,
		string? plainTextContent,
		string? htmlContent)
	{
		var msg = new SendGridMessage
		{
			From = new EmailAddress(fromEmail, fromName),
			Subject = subject,
			PlainTextContent = plainTextContent,
			HtmlContent = htmlContent
		};
		msg.AddTo(new EmailAddress(toEmail, toName));
		return new SendGridClient(apiKey).SendEmailAsync(msg);
	}
}