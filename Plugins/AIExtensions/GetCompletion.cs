using System.ComponentModel;
using Shared;

namespace AIExtensions;

[SpeakUpTool]
public class GetCompletion
{
    [Description("Generates a completion for the given prompt using the OpenAI API.")]
    public static async Task<string> GetCompletionAsync(string apiKey, string prompt, string model)
    {
        var api = new OpenAI.OpenAIClient(apiKey);
        var result = await api.GetChatClient(model).CompleteChatAsync(prompt);
        var output = result.Value.Content.Select(x => x.Text);
        return string.Join(" ", output);
    }
}