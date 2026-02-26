using System.ComponentModel;
using System.Diagnostics;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;

[SpeakUpTool]
public class PhoneValidation
{
	[Description("Validates a phone number using the Abstract API phone validation service.")]
	public static async Task<bool> IsPhoneNumberValid(string apiKey, string phone, CancellationToken cancellationToken)
	{
		Debug.WriteLine($"Calling Abstract API phone validation service with a phone number: {phone} and API key: {apiKey}");
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://phoneintelligence.abstractapi.com/v1/");
		var result = await api.GetAsync($"?api_key={apiKey}&phone={phone}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var content = await result.Content.ReadFromJsonAsync<PhoneValidationResponse>(cancellationToken: cancellationToken);
			return content is { phone_validation.is_valid: true };
		}

		return false;
	}
}