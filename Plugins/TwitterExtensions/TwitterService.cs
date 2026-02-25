namespace TwitterExtensions;

using Tweetinvi;
using Tweetinvi.Models;

public class TwitterService(string? accessToken, string? accessTokenSecret, string? consumerKey, string? consumerSecret)
{
	private readonly TwitterClient _userClient = new(consumerKey, consumerSecret, accessToken, accessTokenSecret);

	public async Task<string> PublishTweet(string? message)
	{
		var tweet = await _userClient.Tweets.PublishTweetAsync(message);
		return tweet.IdStr;
	}

	public Task DestroyTweet(long twitterId)
	{
		return _userClient.Tweets.DestroyTweetAsync(twitterId);
	}

	public Task<ITweet> GetTweet(long tweetId)
	{
		return _userClient.Tweets.GetTweetAsync(tweetId);
	}
}