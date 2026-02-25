namespace TwilioExtensions;

public interface ITwilioService
{
	Task SendSmsAsync(string? accountSid,
		string? authToken,
		string? toPhoneNumber,
		string? fromPhoneNumber,
		string? message);
}