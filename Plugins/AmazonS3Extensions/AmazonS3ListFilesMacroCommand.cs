using System.ComponentModel;
using Shared;

namespace AmazonS3Extensions;

[SpeakUpTool]
public class AmazonS3ListFilesMacroCommand
{
	[Description("Lists files in an Amazon S3 bucket.")]
	public async Task<IEnumerable<S3File>?> ListFiles(string accessKey, string secretKey, string bucketName, CancellationToken cancellationToken)
	{
		using var amazonS3Api = new AmazonS3Api(accessKey, secretKey);
		var listResult = await amazonS3Api.ListFiles(bucketName, cancellationToken);
		if (listResult.Exception is null)
		{
			return listResult.Files;
		}

		return Enumerable.Empty<S3File>();
	}
}