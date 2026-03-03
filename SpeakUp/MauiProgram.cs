using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Media;
using Microsoft.Extensions.Logging;
using SpeakUp.Executor;
using SpeakUp.Pages;
using SpeakUp.Services;

namespace SpeakUp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
         
#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Services
            builder.Services.AddSingleton<ISettingsService, SettingsService>();
            builder.Services.AddSingleton<ICommandHistoryService, CommandHistoryService>();
            builder.Services.AddSingleton<IPluginInfoService, PluginInfoService>();
            builder.Services.AddSingleton<IPluginManagerService, PluginManagerService>();
            builder.Services.AddSingleton<IErrorHandlingService, ErrorHandlingService>();
            builder.Services.AddSingleton<IWorkflowService, WorkflowService>();
            builder.Services.AddSingleton<IWorkflowExecutionService, WorkflowExecutionService>();
            builder.Services.AddSingleton<IExecutor, McpExecutor>();
            
            // Speech to text
            builder.Services.AddKeyedSingleton<ISpeechToText>(nameof(SpeechToTextImplementation), (_, _) => SpeechToText.Default);
            builder.Services.AddKeyedSingleton<ISpeechToText>(nameof(OfflineSpeechToTextImplementation), (_, _) => OfflineSpeechToText.Default);
            
            // ViewModels
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddTransient<SettingsPageViewModel>();
            builder.Services.AddTransient<CommandHistoryPageViewModel>();
            builder.Services.AddTransient<PluginManagerPageViewModel>();
            builder.Services.AddTransient<WorkflowListPageViewModel>();
            builder.Services.AddTransient<WorkflowEditorPageViewModel>();
            
            // Pages
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<CommandHistoryPage>();
            builder.Services.AddTransient<PluginManagerPage>();
            builder.Services.AddTransient<WorkflowListPage>();
            builder.Services.AddTransient<WorkflowEditorPage>();

            return builder.Build();
        }
    }
}
