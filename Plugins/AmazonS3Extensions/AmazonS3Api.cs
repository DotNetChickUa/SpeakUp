namespace AmazonS3Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

public sealed class AmazonS3Api(string accessKey, string secretKey) : IDisposable
{
	private readonly AmazonS3Client _client = new(new BasicAWSCredentials(accessKey, secretKey), RegionEndpoint.EUWest2);

	public void Dispose()
	{
		_client.Dispose();
	}

	public async Task<AmazonS3ListResult> ListFiles(string bucketName, CancellationToken cancellationToken)
	{
		try
		{
			var files = await _client.ListObjectsAsync(new ListObjectsRequest
			{
				BucketName = bucketName
			}, cancellationToken);
			if (files.HttpStatusCode == HttpStatusCode.OK)
			{
				return new AmazonS3ListResult(files.S3Objects.Select(x => new S3File
				{
					Id = x.Key,
					Name = x.Key,
					ModifiedTime = x.LastModified,
					Size = x.Size
				}).ToList(), null);
			}

			return new AmazonS3ListResult(null, new AmazonS3Exception("Error code: {files.HttpStatusCode}"));
		}
		catch (Exception e)
		{
			return new AmazonS3ListResult(null, e);
		}
		
	}
}

public record AmazonS3ListResult(List<S3File>? Files, [property: MemberNotNull("Files")] Exception? Exception);