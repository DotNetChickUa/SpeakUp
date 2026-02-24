namespace AbstractApiExtensions;

public class ExchangeRate
{
	public required string Base { get; set; }

	public long LastUpdated { get; set; }

	public Dictionary<string, double> ExchangeRates { get; set; } = new();
}