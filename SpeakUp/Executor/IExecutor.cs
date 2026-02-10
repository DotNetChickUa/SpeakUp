namespace SpeakUp;

public interface IExecutor
{
    Task<string> Execute(string command);
}