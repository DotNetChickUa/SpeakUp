using OpenAI;
using System.ClientModel;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using Shared;

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
        var tools = GetTools();

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

    private IList<AITool> GetTools()
    {
        var pluginFiles = Directory.GetFiles("Plugins", "*.dll", SearchOption.AllDirectories);
        var tools = new List<AITool>();
        foreach (var pluginFile in pluginFiles)
        {
            var assembly = System.Reflection.Assembly.LoadFrom(pluginFile);
            var types = assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(SpeakUpToolAttribute), false).Length > 0);
            foreach (var type in types)
            {
                var methods = type.GetMethods().Where(m => m.IsPublic && m.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0);
                foreach (var method in methods)
                {
                    var tool = AIFunctionFactory.Create(method, target: null, name: method.Name, method.GetCustomAttribute<DescriptionAttribute>()?.Description);
                    tools.Add(tool);
                }
            }
        }

        return tools;
    }
}