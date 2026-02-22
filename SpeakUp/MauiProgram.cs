using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpeakUp.Executor;

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
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IExecutor, McpExecutor>();
            builder.Services.AddKeyedSingleton<ISpeechToText>(nameof(SpeechToTextImplementation), (_, _) => SpeechToText.Default);
            builder.Services.AddKeyedSingleton<ISpeechToText>(nameof(OfflineSpeechToTextImplementation), (_, _) => OfflineSpeechToText.Default);
            builder.Services.AddSingleton<MainPageViewModel>();
            return builder.Build();
        }
    }
}
