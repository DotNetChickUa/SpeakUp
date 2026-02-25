using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Base.Extensions.HTTP;

public abstract class HttpMacroCommand
{
	protected static readonly JsonSerializerOptions Settings = new()
	{
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
	};
}