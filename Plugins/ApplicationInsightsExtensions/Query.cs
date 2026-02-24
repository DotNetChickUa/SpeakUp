using System.ComponentModel;
using Shared;

namespace ApplicationInsightsExtensions;

using System.Text.Json;

[SpeakUpTool]
public class Query : AppInsightMacroCommand
{
	[Description("The query to execute against Application Insights")]
	public async Task<object?> GetResultsAsync(string appId, string query, string apiKey, CancellationToken cancellationToken)
	{
		var url = BuildUrl(appId, query);
		using var client = BuildClient(apiKey);
		var response = await client.GetAsync(url, cancellationToken);
		if (!response.IsSuccessStatusCode)
		{
			return null;
		}

		var json = await response.Content.ReadAsStreamAsync(cancellationToken);
		return JsonSerializer.DeserializeAsync<object>(json, cancellationToken: cancellationToken);
	}
}