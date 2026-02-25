using System.ComponentModel;
using System.Net.Http.Json;
using Shared;

namespace ElasticEmailExtensions;

[SpeakUpTool]
public class SendEmail
{
    [Description("Sends email using ElasticEmail")]
    public static async Task<string> SendEmailAsync(string apiKey, string to, string from, string subject, string message, CancellationToken cancellationToken)
    {
        var values = new Dictionary<string, string?>
        {
            {
                "apikey", apiKey
            },
            {
                "from", from
            },
            {
                "fromName", "Your Company Name"
            },
            {
                "to", to
            },
            {
                "subject", subject
            },
            {
                "bodyText", message
            }
        };

        using var client = new HttpClient();

        var response = await client.PostAsync("https://api.elasticemail.com/v2/email/send",
                                              new FormUrlEncodedContent(values), cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var result =
                await response.Content.ReadFromJsonAsync<ElasticEmailResultModel>(cancellationToken: cancellationToken);
            if (result is null)
            {
                return "Unable to retrieve result.";
            }

            if (result.Success)
            {
                return $"Email sent to {to}.";
            }

            return $"Email is not sent. {result.Error}";
        }

        return "Invalid request. Email is not sent.";
    }
}