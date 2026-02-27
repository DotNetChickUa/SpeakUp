using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using Shared;
using System.ClientModel;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;

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

            AgentSession session = await agent.CreateSessionAsync();
            var result = new StringBuilder();

            await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(command, session))
            {
                foreach (AIContent content in update.Contents)
                {
                    if (content is FunctionCallContent functionCall)
                    {
                        await Application.Current.Dispatcher.DispatchAsync(async () =>
                        {
                            await Shell.Current.CurrentPage.DisplayAlertAsync("Workflow Request", $"Workflow requests input: {functionCall.Name}", "OK");
                            foreach (var argumentKey in functionCall.Arguments.Keys)
                            {
                                var existingValue = functionCall.Arguments[argumentKey];
                                var promptValue = await Shell.Current.CurrentPage.DisplayPromptAsync(
                                    "Workflow Request",
                                    $"Please provide a value for {argumentKey}",
                                    "OK",
                                    initialValue: existingValue?.ToString());

                                functionCall.Arguments[argumentKey] = GetArgumentValueForInput(existingValue, promptValue);
                            }
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
                //Endpoint = new Uri("https://api.chatanywhere.tech/v1")
                Endpoint = new Uri("https://free.v36.cm/v1")
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
                        var tool = AIFunctionFactory.Create(method, target: null, name: $"{string.Join("", type.FullName.TakeLast(40))}.{method.Name}", description);
                        tools.Add(tool);
                        _logger.LogInformation("Loaded tool: {ToolName} from {Assembly}", tool.Name, assembly.FullName);
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

    private static object? GetArgumentValueForInput(object? existingValue, string? input)
    {
        if (input is null)
        {
            return existingValue;
        }

        if (existingValue is null)
        {
            return input;
        }

        if (existingValue is JsonElement jsonElement)
        {
            return GetValueFromJsonElement(jsonElement, input);
        }

        var targetType = existingValue.GetType();
        if (targetType == typeof(string))
        {
            return input;
        }

        if (targetType.IsEnum && Enum.TryParse(targetType, input, ignoreCase: true, out var enumValue))
        {
            return enumValue;
        }

        try
        {
            return Convert.ChangeType(input, targetType, CultureInfo.InvariantCulture);
        }
        catch (InvalidCastException)
        {
            return input;
        }
        catch (FormatException)
        {
            return input;
        }
        catch (OverflowException)
        {
            return input;
        }
    }

    private static JsonElement GetValueFromJsonElement(JsonElement element, string input)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Number:
                if (long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                {
                    return JsonSerializer.SerializeToElement(longValue);
                }

                if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
                {
                    return JsonSerializer.SerializeToElement(doubleValue);
                }

                return JsonSerializer.SerializeToElement(input);
            case JsonValueKind.True:
            case JsonValueKind.False:
                if (bool.TryParse(input, out var boolValue))
                {
                    return JsonSerializer.SerializeToElement(boolValue);
                }

                return JsonSerializer.SerializeToElement(input);
            case JsonValueKind.Object:
            case JsonValueKind.Array:
                if (TryParseJsonElement(input, out var parsedJson))
                {
                    return parsedJson;
                }

                return JsonSerializer.SerializeToElement(input);
            case JsonValueKind.String:
            default:
                return JsonSerializer.SerializeToElement(input);
        }
    }

    private static bool TryParseJsonElement(string input, out JsonElement element)
    {
        try
        {
            using var document = JsonDocument.Parse(input);
            element = document.RootElement.Clone();
            return true;
        }
        catch (JsonException)
        {
            element = default;
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