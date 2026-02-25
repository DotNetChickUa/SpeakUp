namespace TwilioExtensions;

using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public class TwilioService : ITwilioService
{
	public async Task SendSmsAsync(string? accountSid,
		string? authToken,
		string? toPhoneNumber,
		string? fromPhoneNumber,
		string? message)
	{
		ArgumentException.ThrowIfNullOrEmpty(accountSid);
		ArgumentException.ThrowIfNullOrEmpty(authToken);
		ArgumentException.ThrowIfNullOrEmpty(message);
		ArgumentException.ThrowIfNullOrEmpty(fromPhoneNumber);
		ArgumentException.ThrowIfNullOrEmpty(toPhoneNumber);
		TwilioClient.Init(accountSid, authToken);
		await MessageResource.CreateAsync(body: message, from: new PhoneNumber(ConvertPhoneNumber(fromPhoneNumber)),
		                                  to: new PhoneNumber(ConvertPhoneNumber(toPhoneNumber)));
		TwilioClient.Invalidate();
	}

	private static string ConvertPhoneNumber(string number)
	{
		if (number.StartsWith('+'))
		{
			return number;
		}

		if (!number.StartsWith('1'))
		{
			return $"+1{number}";
		}

		return $"+{number}";
	}
}