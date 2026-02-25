using System.ComponentModel;
using Shared;

namespace Base.Extensions.HTTP;

[SpeakUpTool]
public class Get : HttpMacroCommand
{
    [Description("Retrieves data from the specified URL using an HTTP GET request.")]
    public static async Task<string?> RetrieveData(Uri url, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        var result = await client.GetAsync(url, cancellationToken);
        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadAsStringAsync(cancellationToken);
        }

        return null;
    }
}