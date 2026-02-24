using System.ComponentModel;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;

[SpeakUpTool]
public class PhoneValidation
{
	[Description("Validates a phone number using the Abstract API phone validation service.")]
	public async Task<bool> IsValid(string apiKey, string phone, CancellationToken cancellationToken)
	{
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://phonevalidation.abstractapi.com/v1/");
		var result = await api.GetAsync($"?api_key={apiKey}&phone={phone}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var content = await result.Content.ReadFromJsonAsync<PhoneValidationResponse>(cancellationToken: cancellationToken);
			return content is { Valid: true };
		}

		return false;
	}
}