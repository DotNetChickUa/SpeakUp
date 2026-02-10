using OpenAI;
using System.ClientModel;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using Shared;

namespace SpeakUp;

internal class McpExecutor(IConfiguration configuration) : IExecutor
{
    public async Task<string> Execute(string command)
    {
        var loadResult = LoadTools();

        try
        {
            var agent = new OpenAIClient(
                    new ApiKeyCredential(configuration["AIKey"]),
                    new OpenAIClientOptions()
                    {
                        Endpoint = new Uri("https://api.chatanywhere.tech/v1")
                    }).GetChatClient("gpt-4o-mini")
                .CreateAIAgent(
                    instructions: "You are a powerful super user that can execute any commands",
                    name: "McpExecutor",
                    tools: loadResult.Tools);

            var response = await agent.RunAsync(command);
            return response.Text;
        }
        finally
        {
            loadResult.Unload();
        }
    }

    private static PluginLoadResult LoadTools()
    {
        var pluginsPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
        var pluginFiles = Directory.Exists(pluginsPath)
            ? Directory.GetFiles(pluginsPath, "*.dll", SearchOption.AllDirectories)
            : Array.Empty<string>();
        var tools = new List<AITool>();
        var loadContexts = new List<PluginLoadContext>();
        foreach (var pluginFile in pluginFiles)
        {
            if (!IsManagedAssembly(pluginFile))
            {
                continue;
            }

            try
            {
                var loadContext = new PluginLoadContext(pluginFile);
                var assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(pluginFile));
                var types = assembly.GetTypes()
                    .Where(t => t.GetCustomAttributes(typeof(SpeakUpToolAttribute), false).Length > 0);
                foreach (var type in types)
                {
                    var methods = type.GetMethods().Where(m =>
                        m.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0);
                    foreach (var method in methods)
                    {
                        var tool = AIFunctionFactory.Create(method, target: null, name: method.Name,
                            method.GetCustomAttribute<DescriptionAttribute>()?.Description);
                        tools.Add(tool);
                    }
                }

                loadContexts.Add(loadContext);
            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine(e);
                foreach (var loaderException in e.LoaderExceptions)
                {
                    Console.WriteLine(loaderException);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
            catch (FileLoadException e)
            {
                Console.WriteLine(e);
            }
            catch (BadImageFormatException e)
            {
                Console.WriteLine(e);
            }
        }

        return new PluginLoadResult(tools, loadContexts);
    }

    private static bool IsManagedAssembly(string path)
    {
        try
        {
            _ = AssemblyName.GetAssemblyName(path);
            return true;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        catch (FileLoadException)
        {
            return false;
        }
        catch (FileNotFoundException)
        {
            return false;
        }
    }
}