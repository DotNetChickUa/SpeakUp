namespace AbstractApiExtensions;

public class ExchangeRateLookupResult(string message)
{
	public override string ToString()
	{
		return message;
	}
}