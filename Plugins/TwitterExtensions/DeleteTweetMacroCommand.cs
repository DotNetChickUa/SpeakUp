using System.ComponentModel;
using Shared;

namespace TwitterExtensions;

[SpeakUpTool]
public class DeleteTweetMacroCommand
{
	[Description("Deletes a tweet using the provided Twitter credentials and tweet ID.")]
	public static async Task Delete(string accessToken, string accessTokenSecret, string consumerKey, string consumerKeySecret, long tweetId, CancellationToken cancellationToken)
	{
		var twitterService = new TwitterService(accessToken, accessTokenSecret, consumerKey, consumerKeySecret);
		await twitterService.DestroyTweet(tweetId);
	}
}