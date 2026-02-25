using System.ComponentModel;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

[SpeakUpTool]
public class IbanValidation
{
	[Description("Validates the given IBAN using the Abstract API IBAN validation service.")]
	public static async Task<bool> IsValid(string apiKey, string iban, CancellationToken cancellationToken)
	{
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://ibanvalidation.abstractapi.com/v1/");
		var result = await api.GetAsync($"?api_key={apiKey}&iban={iban}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			};
			options.Converters.Add(new JsonStringEnumConverter());
			var content = await result.Content.ReadFromJsonAsync<IbanValidationResponse>(options, cancellationToken);
			return content is { IsValid: true };
		}

		return false;
	}
}