namespace AbstractApiExtensions;

public class VatValidationResponse
{
	public string? VatNumber { get; set; }

	public bool Valid { get; set; }
}