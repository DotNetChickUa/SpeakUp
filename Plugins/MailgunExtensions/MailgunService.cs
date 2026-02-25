namespace MailgunExtensions;

using System.Net.Http.Headers;
using System.Text;

public sealed class MailgunService : IMailgunService
{
	private const string MailgunBaseUri = "https://api.mailgun.net/v3/";

	private readonly HttpClient client = new();

	public Task SendEmailAsync(string? apiKey,
		Uri? requestUri,
		string? to,
		string? from,
		string? subject,
		string? message)
	{
		ArgumentException.ThrowIfNullOrEmpty(apiKey);
		ArgumentException.ThrowIfNullOrEmpty(to);
		ArgumentException.ThrowIfNullOrEmpty(from);
		ArgumentException.ThrowIfNullOrEmpty(subject);
		ArgumentException.ThrowIfNullOrEmpty(message);

		client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}")));

		var content = new MultipartFormDataContent();

		var values = new List<KeyValuePair<string, string>>
		{
			new("from", from),
			new("to", to),
			new("subject", subject),
			new("text", message)
		};

		values.ForEach(v => content.Add(new StringContent(v.Value), v.Key));

		return client.PostAsync($"{MailgunBaseUri}{requestUri}/messages", content);
	}

	public void Dispose()
	{
		client.Dispose();
	}
}