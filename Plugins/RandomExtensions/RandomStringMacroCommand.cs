using System.ComponentModel;
using Shared;

namespace RandomExtensions;

using System.Security.Cryptography;

[SpeakUpTool]
public class RandomStringMacroCommand
{
	private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

	[Description("Generates a random string of specified length using allowed characters.")]
	public static async Task<string> GenerateRandomString(int length, string allowedChars, CancellationToken cancellationToken)
	{
		if (length < 0)
		{
			length = 0;
		}

		if (string.IsNullOrEmpty(allowedChars))
		{
			allowedChars = Chars;
		}

		return GenerateRandomString(length, allowedChars);
	}

	private static string GenerateRandomString(int length, string allowedChars)
	{
		using var rng = RandomNumberGenerator.Create();
		var bytes = new byte[length];
		var result = new char[length];
		var allowedCharsLength = allowedChars.Length;

		rng.GetBytes(bytes);
		for (var i = 0; i < length; i++)
		{
			var index = bytes[i] % allowedCharsLength;
			result[i] = allowedChars[index];
		}

		return new string(result);
	}
}