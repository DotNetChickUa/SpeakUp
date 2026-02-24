using System.ComponentModel;
using Shared;

namespace AIExtensions;

using System.Net.Http.Json;
[SpeakUpTool]
public class PlagiarismChecker
{
	[Description("Checks the provided text for plagiarism using the Edubirdie plagiarism checker.")]
	public  async Task<PlagiarismResponse?> GetCompletionAsync(string text, 
        CancellationToken cancellationToken)
	{
		using var httpClient = new HttpClient();
		var formData = new List<KeyValuePair<string, string>>
		{
			new("is_free", "true"),
			new("title", string.Empty),
			new("text", text)
		};
		httpClient.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
		var response = await httpClient.PostAsync("https://edubirdie.com/plagiarism-checker-send-data",
		                                          new FormUrlEncodedContent(formData), cancellationToken);
		var plagiarismResult =
			await response.Content.ReadFromJsonAsync<PlagiarismResponse>(cancellationToken: cancellationToken);

		return plagiarismResult;
	}
}

public class PlagiarismResponse
{
	public string? Error { get; set; }
	public int ErrorCode { get; set; }
	public string? Text { get; set; }
	public string? Percent { get; set; }
	public object[]? Highlight { get; set; }
	public object[]? Matches { get; set; }
	public string? Title { get; set; }
	public string? Message { get; set; }
	public int WordsCount { get; set; }
}