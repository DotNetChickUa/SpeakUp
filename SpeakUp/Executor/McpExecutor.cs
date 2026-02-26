using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using Shared;
using System.ClientModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Microsoft.Agents.AI.Workflows;

namespace SpeakUp.Executor;

internal class McpExecutor : IExecutor, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<McpExecutor> _logger;
    private readonly Lazy<PluginLoadResult> _lazyPlugins;
    private ChatClient? _chatClient;
    private bool _disposed;

    public McpExecutor(IConfiguration configuration, ILogger<McpExecutor> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _lazyPlugins = new Lazy<PluginLoadResult>(LoadTools);
    }

    public async Task<string> Execute(string command)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        try
        {
            var tools = _lazyPlugins.Value.Tools;
            var agent = GetOrCreateChatClient()
                .AsIChatClient()
                .AsAIAgent(
                    instructions: "You are a powerful super user that can execute any commands. It is important you do exactly what I ask you. Make sure you run the command in correct order, with correct arguments and ensure you don't replay the same command multiple times. You must follow the workflow I provided for you. Example: I ask you to start notepad and enter the text 'Hello World'. You must start the notepad only once, and enter exactly the text 'Hello World'",
                    name: "McpExecutor",
                    tools: tools);

            var workflow = AgentWorkflowBuilder.BuildSequential(agent);
            AIAgent workflowAgent = workflow.AsAIAgent(
                id: "content-pipeline",
                name: "Content Pipeline Agent",
                description: "A multi-agent workflow that researches, writes, and reviews content"
            );
            // Create a new session for the conversation
            AgentSession session = await workflowAgent.CreateSessionAsync();
            var result = new StringBuilder();

            await foreach (AgentResponseUpdate update in workflowAgent.RunStreamingAsync(command, session))
            {
                foreach (AIContent content in update.Contents)
                {
                    if (content is FunctionCallContent functionCall)
                    {
                        functionCall.AdditionalProperties ??= new AdditionalPropertiesDictionary();
                        await Application.Current.Dispatcher.DispatchAsync<string>(async () =>
                        {
                            await Shell.Current.CurrentPage.DisplayAlertAsync("Workflow Request", $"Workflow requests input: {functionCall.Name}", "OK");
                            foreach (var argument in functionCall.Arguments)
                            {
                                functionCall.Arguments[argument.Key] = await Shell.Current.CurrentPage.DisplayPromptAsync("Workflow Request", $"Please provide a value for {argument.Key}", "OK", initialValue: argument.Value.ToString());
                            }

                            return string.Empty;
                        });
                    }
                    else if (content is TextContent textContent)
                    {
                        result.Append(textContent.Text);
                    }
                }
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command: {Command}", command);
            return $"Execution failed: {ex.Message}";
        }
    }

    private ChatClient GetOrCreateChatClient()
    {
        if (_chatClient is not null)
        {
            return _chatClient;
        }

        _chatClient = new OpenAIClient(
            new ApiKeyCredential(_configuration["AIKey"] ?? throw new InvalidOperationException("AIKey not configured")),
            new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://api.chatanywhere.tech/v1")
            }).GetChatClient("gpt-4o-mini");
        return _chatClient;
    }

    private PluginLoadResult LoadTools()
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
                    .Where(t => t.GetCustomAttribute<SpeakUpToolAttribute>() is not null);

                foreach (var type in types)
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(m => m.GetCustomAttribute<DescriptionAttribute>() is not null);

                    foreach (var method in methods)
                    {
                        var description = method.GetCustomAttribute<DescriptionAttribute>()?.Description;
                        var tool = AIFunctionFactory.Create(method, target: null, name: method.Name, description);
                        tools.Add(tool);
                        _logger.LogInformation("Loaded tool: {ToolName} from {Assembly}", method.Name, assembly.FullName);
                    }
                }

                loadContexts.Add(loadContext);
            }
            catch (ReflectionTypeLoadException e)
            {
                _logger.LogError(e, "ReflectionTypeLoadException loading {PluginFile}", pluginFile);
                if (e.LoaderExceptions is not null)
                {
                    foreach (var loaderException in e.LoaderExceptions)
                    {
                        _logger.LogError(loaderException, "Loader exception");
                    }
                }
            }
            catch (Exception e) when (e is FileNotFoundException or FileLoadException or BadImageFormatException)
            {
                _logger.LogWarning(e, "Could not load plugin {PluginFile}", pluginFile);
            }
        }

        _logger.LogInformation("Loaded {Count} tools from {ContextCount} plugin contexts", tools.Count, loadContexts.Count);
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

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_lazyPlugins.IsValueCreated)
        {
            _lazyPlugins.Value.Unload();
        }

        _disposed = true;
    }
}