namespace SpeakUp.Executor;

public interface IExecutor
{
    Task<string> Execute(string command);
}