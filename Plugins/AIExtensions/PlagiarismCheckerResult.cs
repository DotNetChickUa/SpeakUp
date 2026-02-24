namespace AIExtensions;

public class PlagiarismCheckerResult(string message)
{
	public override string ToString()
	{
		return message;
	}
}