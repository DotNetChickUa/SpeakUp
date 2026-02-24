namespace AbstractApiExtensions;

public class VatValidationResult(string message)
{
    public override string ToString()
    {
        return message;
    }
}