using System.ComponentModel;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

[SpeakUpTool]
public class VatValidation
{
	[Description("Validates a VAT number using the Abstract API VAT validation service.")]
	public static async Task<bool> IsVatValid(string apiKey, string vatNumber, CancellationToken cancellationToken)
	{
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://vat.abstractapi.com/v1/validate/");
		var result = await api.GetAsync($"?api_key={apiKey}&vat_number={vatNumber}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			};
			options.Converters.Add(new JsonStringEnumConverter());
			var content = await result.Content.ReadFromJsonAsync<VatValidationResponse>(options, cancellationToken);
			return content is { Valid: true };
		}

		return false;
	}
}