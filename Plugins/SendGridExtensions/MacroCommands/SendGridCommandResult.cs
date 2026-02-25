namespace SendGridExtensions.MacroCommands;

using DrawGo.Plugin.Base.Results;

public class SendGridCommandResult(string message) : HasErrorResult
{
	public override string ToString()
	{
		return $"Message: {message}";
	}
}