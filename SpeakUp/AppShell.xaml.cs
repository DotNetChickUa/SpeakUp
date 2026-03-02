using SpeakUp.Pages;

namespace SpeakUp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Register navigation routes
            Routing.RegisterRoute("WorkflowEditor", typeof(WorkflowEditorPage));
        }
    }
}
