using System.ComponentModel;
using Shared;

namespace FacebookExtensions;

[SpeakUpTool]
public class CreatePostMacroCommand
{
	[Description("Creates a new Facebook post with the specified text.")]
    public static async Task<string?> Post(string accessToken, string text, CancellationToken cancellationToken)
	{
		var postId = await FacebookService.NewPost(accessToken, text, cancellationToken);
		var postId2 = await FacebookService.NewPost2(accessToken, text, cancellationToken);
		return postId;
	}
}