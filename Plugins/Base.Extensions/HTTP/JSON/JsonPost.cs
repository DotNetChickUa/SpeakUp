using System.ComponentModel;
using System.Text;
using System.Text.Json;
using Shared;

namespace Base.Extensions.HTTP.JSON;

[SpeakUpTool]
public class JsonPost : HttpMacroCommand
{
	[Description("Sends a POST request with a JSON body to the specified URL.")]
	public static async Task<object?> MakePostAsync(Uri url, object jsonBody, CancellationToken cancellationToken)
	{
		var jsonString = jsonBody as string ?? JsonSerializer.Serialize(jsonBody, Settings);
		using var client = new HttpClient();
		var result = await client.PostAsync(url,
		                                    new StringContent(jsonString, Encoding.UTF8, "application/json"),
		                                    cancellationToken);
		return await JsonSerializer.DeserializeAsync<object>(await result.Content.ReadAsStreamAsync(cancellationToken),
		                                                     Settings, cancellationToken);
	}
}