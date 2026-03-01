using Microsoft.Extensions.DependencyInjection;

namespace SpeakUp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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
    }
}