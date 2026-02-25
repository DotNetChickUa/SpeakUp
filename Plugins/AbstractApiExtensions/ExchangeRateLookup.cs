using System.ComponentModel;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

[SpeakUpTool]
public class ExchangeRateLookup
{
	[Description("Retrieves the exchange rate between two currencies using the Abstract API exchange rate service.")]
	public static async Task<ExchangeRate?> IsValid(string apiKey, string @base, string target, CancellationToken cancellationToken)
	{
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://exchange-rates.abstractapi.com/v1/live/");
		var result = await api.GetAsync($"?api_key={apiKey}&base={@base}&target={target}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			};
			options.Converters.Add(new JsonStringEnumConverter());
			var content = await result.Content.ReadFromJsonAsync<ExchangeRate>(options, cancellationToken);
			return content;
		}

		return null;
	}
}