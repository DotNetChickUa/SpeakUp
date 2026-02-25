namespace SlackExtensions;

using SlackNet;
using SlackNet.WebApi;

public static class SlackService
{
	public static async Task<string> NewMessage(string? token,
		string? channel,
		string? message,
		CancellationToken cancellationToken)
	{
		var api = new SlackServiceBuilder()
		          .UseApiToken(token)
		          .GetApiClient();
		var response = await api.Chat.PostMessage(new Message { Text = message, Channel = channel }, cancellationToken);
		return response.Message.ClientMsgId.ToString();
	}
}