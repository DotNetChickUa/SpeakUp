using Microsoft.Extensions.DependencyInjection;
using SpeakUp.Services;

namespace SpeakUp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            ApplyStartupTheme();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            
#if WINDOWS
            const int width = 500;
            const int height = 800;
            window.Width = width;
            window.Height = height;
            window.MaximumWidth = width;
            window.MaximumHeight = height;
            window.MinimumWidth = width;
            window.MinimumHeight = height;
#endif

#if MACCATALYST
            const int width = 500;
            const int height = 800;
            window.Width = width;
            window.Height = height;
            window.MaximumWidth = width;
            window.MaximumHeight = height;
            window.MinimumWidth = width;
            window.MinimumHeight = height;
#endif
            
            return window;
        }

        private static void ApplyStartupTheme()
        {
            var services = IPlatformApplication.Current?.Services;
            if (services is null || Application.Current is null)
            {
                return;
            }

            var settingsService = services.GetService<ISettingsService>();
            if (settingsService is null)
            {
                return;
            }

            var settings = settingsService.LoadSettingsAsync().GetAwaiter().GetResult();
            Application.Current.UserAppTheme = settings.General.Theme?.ToLowerInvariant() switch
            {
                "light" => AppTheme.Light,
                "dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified
            };
        }
    }
}