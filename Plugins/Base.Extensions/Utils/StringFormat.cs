using System.ComponentModel;
using Shared;

namespace Base.Extensions.Utils;

[SpeakUpTool]
public class StringFormat
{
	[Description("Formats a string using the specified format and arguments.")]
	public static async Task<string> FormatString(string format, params object[] strings)
	{
		return string.Format(format, strings.ToArray());
	}
}