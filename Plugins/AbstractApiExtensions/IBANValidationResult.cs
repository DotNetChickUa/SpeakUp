namespace AbstractApiExtensions;

public class IbanValidationResult(string message)
{
	public override string ToString()
	{
		return message;
	}
}