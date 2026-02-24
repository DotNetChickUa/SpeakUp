using System.ComponentModel;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

[SpeakUpTool]
public class IpGeolocationLookup
{
	[Description("Retrieves geolocation information for the given IP address using the Abstract API IP geolocation service.")]
	public async Task<IpGeolocationLookupResponse?> IsValid(string apiKey, CancellationToken cancellationToken)
	{
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://ipgeolocation.abstractapi.com/v1/");
		var result = await api.GetAsync($"?api_key={apiKey}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			};
			options.Converters.Add(new JsonStringEnumConverter());
			var content = await result.Content.ReadFromJsonAsync<IpGeolocationLookupResponse>(options, cancellationToken);
			return content;
		}

		return null;
	}
}