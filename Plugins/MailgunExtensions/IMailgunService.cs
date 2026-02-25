namespace MailgunExtensions;

public interface IMailgunService : IDisposable
{
	Task SendEmailAsync(string? apiKey, Uri? requestUri, string? to, string? from, string? subject, string? message);
}