using System.ComponentModel;
using Shared;

namespace AbstractApiExtensions;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

[SpeakUpTool]
public class HolidaysLookup
{
	[Description("Checks if the given date is a holiday in the specified country using the Abstract API.")]
	public static async Task<Holiday[]?> GetHolidays(string apiKey, string country, DateTime date, CancellationToken cancellationToken)
	{
		using var api = new HttpClient();
		api.BaseAddress = new Uri("https://holidays.abstractapi.com/v1/");
		var result = await api.GetAsync($"?api_key={apiKey}&country={country}&year={date.Year}&month={date.Month}&day={date.Day}", cancellationToken);

		if (result.IsSuccessStatusCode)
		{
			var options = new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
				PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
			};
			options.Converters.Add(new JsonStringEnumConverter());
			var content = await result.Content.ReadFromJsonAsync<Holiday[]>(options, cancellationToken);
			return content;
		}

		return null;
	}
}