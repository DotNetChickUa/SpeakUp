using System.ComponentModel;
using System.Net.Http.Json;
using Shared;

namespace Base.Extensions.HTTP.JSON;

[SpeakUpTool]
public class JsonGet : HttpMacroCommand
{
	[Description("Retrieves data from the specified URL using an HTTP GET request and parses it as JSON.")]
	public static async Task<object?> RetrieveData(Uri url, CancellationToken cancellationToken)
	{
		using var client = new HttpClient();
		return await client.GetFromJsonAsync<object?>(url, Settings, cancellationToken);
	}
}