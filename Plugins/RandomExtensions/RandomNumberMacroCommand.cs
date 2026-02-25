using System.ComponentModel;
using Shared;

namespace RandomExtensions;

using System.Security.Cryptography;

[SpeakUpTool]
public class RandomNumberMacroCommand
{
	[Description("Generates a random number between the specified minimum and maximum values.")]
	public static async Task<int> GenerateRandomNumber(int minValue, int maxValue)
	{
		if (minValue == maxValue)
		{
			return minValue;
		}

		if (minValue > maxValue)
		{
			(minValue, maxValue) = (maxValue, minValue);
		}

		return RandomNumberGenerator.GetInt32(minValue, maxValue);
	}
}