namespace AbstractApiExtensions;

public class EmailValidationResult(string message)
{
	public override string ToString()
	{
		return message;
	}
}