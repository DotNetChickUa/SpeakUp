namespace ApplicationInsightsExtensions;

using System.Net.Http.Headers;
using System.Web;
public abstract class AppInsightMacroCommand
{
	protected const string AppInsightUrl = "https://api.applicationinsights.io/v1/apps/{0}/query?query={1}";

	protected static HttpClient BuildClient(string? apiKey)
	{
		var client = new HttpClient();
		client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		client.DefaultRequestHeaders.Add("x-api-key", apiKey);
		return client;
	}

	protected static string BuildUrl(string? appId, string? query, string? timespan = null)
	{
		var url = string.Format(AppInsightUrl, appId, HttpUtility.UrlEncode(query));
		if (!string.IsNullOrEmpty(timespan))
		{
			url += $"&timespan={timespan}";
		}

		return url;
	}
}