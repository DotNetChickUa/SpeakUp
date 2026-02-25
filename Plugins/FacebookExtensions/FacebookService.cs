using System.Net.Http.Json;

namespace FacebookExtensions;

using System.Text;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

public static class FacebookService
{
	public static async Task<string?> NewPost(string? accessToken, string? message, CancellationToken cancellationToken)
	{
		using var client = new HttpClient();

		var url = $"https://graph.facebook.com/v12.0/me/feed?access_token={accessToken}";

		dynamic postData = new
		{
			message
		};

		var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

		var response = await client.PostAsync(url, content, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			return null;
		}

		var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
		dynamic? result = JsonConvert.DeserializeObject(responseBody);
		return result?.id;
	}

	public static async Task<string?> NewPost2(string? accessToken, string? message, CancellationToken cancellationToken)
	{
		using var client = new HttpClient();

		var url = $"https://graph.facebook.com/v12.0/me/feed?access_token={accessToken}";

		var postData = new
		{
			message
		};

		var response = await client.PostAsJsonAsync(url, postData, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			return null;
		}

		var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
		dynamic? result = JsonSerializer.Deserialize<object?>(responseBody);
		return result?.id;
	}
}