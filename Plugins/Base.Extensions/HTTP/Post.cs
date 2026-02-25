using System.ComponentModel;
using System.Text;
using Shared;

namespace Base.Extensions.HTTP;

[SpeakUpTool]
public class Post : HttpMacroCommand
{
	[Description("Sends a POST request to the specified URL with the given body and media type.")]
	public static async Task<string> GetPostResponse(Uri url, string body, string mediaType, CancellationToken cancellationToken)
	{
		using var client = new HttpClient();
		var response = await client.PostAsync(url, new StringContent(body, Encoding.Default, mediaType),
		                                      cancellationToken);

		return await response.Content.ReadAsStringAsync(cancellationToken);
	}
}