using System.ComponentModel;
using OpenAI.Images;
using Shared;

namespace AIExtensions;

[SpeakUpTool]
public class ImageGeneration
{
	[Description("Generates an image based on the given prompt using the OpenAI API.")]
	public async Task<Uri> GetCompletionAsync(string apiKey, string prompt, string model, CancellationToken cancellationToken)
	{
		var api = new OpenAI.OpenAIClient(apiKey);
		var result = await api.GetImageClient(model).GenerateImageAsync(prompt, new ImageGenerationOptions(){ResponseFormat = GeneratedImageFormat.Uri}, cancellationToken: cancellationToken);

		return result.Value.ImageUri;
	}
}