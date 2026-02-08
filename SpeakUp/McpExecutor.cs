using OpenAI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using SpeakUp.Tools;

namespace SpeakUp;

public interface IExecutor
{
    Task<string> Execute(string command);
}

internal class McpExecutor : IExecutor
{
    /// <inheritdoc />
    public async Task<string> Execute(string command)
    {
        AITool[] tools =
        [
            AIFunctionFactory.Create(ProcessTools.GetProcesses),
            AIFunctionFactory.Create(ProcessTools.RunApp),
            AIFunctionFactory.Create(ProcessTools.CloseApp),
            AIFunctionFactory.Create(HookTools.EnterText),
            AIFunctionFactory.Create(HookTools.MoveMouse),
        ];

        var agent = new OpenAIClient(
            new ApiKeyCredential(""),
            new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://api.chatanywhere.tech/v1")
            }).GetChatClient("gpt-4o-mini")
            .CreateAIAgent(
                instructions: "You are a powerful super user that can execute any commands", 
                name: "McpExecutor",
                tools: tools);

        var response = await agent.RunAsync(command);
        return response.Text;
    }
}