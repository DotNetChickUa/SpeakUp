using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared;

namespace Base.Extensions.Utils;

[SpeakUpTool]
public class ToJson
{
    private static readonly JsonSerializerOptions SerializerSettings = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    [Description("Converts an object to its JSON representation.")]
    public static string GetJson(object value)
    {
        return JsonSerializer.Serialize(value, SerializerSettings);
    }
}