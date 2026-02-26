using System.ComponentModel;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

[SpeakUpTool]
public class EmailValidation
{
	[Description("Validates an email address using the Abstract API email verification service.")]
	public static async Task<bool> IsEmailValid(string apiKey, string email, CancellationToken cancellationToken)
	{
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://emailvalidation.abstractapi.com/v1/");
		var result = await api.GetAsync($"?api_key={apiKey}&email={email}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var options = new JsonSerializerOptions();
			options.Converters.Add(new JsonStringEnumConverter());
			var content = await result.Content.ReadFromJsonAsync<EmailValidationResponse>(options, cancellationToken);
			return content is { Deliverability: Deliverability.Deliverable };
		}

		return false;
	}
}