using System.ComponentModel;
using Shared;

namespace Base.Extensions.Utils;

[SpeakUpTool]
public class Concat
{
	[Description("Concatenates two strings together.")]
	public static async Task<string> ConcatString(string a, string b)
    {
		return a + b;
	}
}