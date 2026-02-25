using System.ComponentModel;
using Shared;

namespace TwitterExtensions;

[SpeakUpTool]
public class PublishTweetMacroCommand
{
	[Description("Publishes a tweet using the provided Twitter credentials and text.")]
	public static async Task<string> Post(string accessToken, string accessTokenSecret, string consumerKey, string consumerKeySecret, string text)
	{
		var twitterService = new TwitterService(accessToken, accessTokenSecret, consumerKey, consumerKeySecret);
		var tweetId = await twitterService.PublishTweet(text);
		return tweetId;
	}
}