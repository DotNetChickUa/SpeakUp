namespace AbstractApiExtensions;

public class EmailValidationResponse
{
	public Deliverability Deliverability { get; set; }
}

public enum Deliverability
{
	Deliverable,
	Undeliverable,
	Risky,
	Unknown
}