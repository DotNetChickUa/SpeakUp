namespace AbstractApiExtensions;

public class PhoneValidationResult(string message)
{
	public override string ToString()
	{
		return message;
	}
}