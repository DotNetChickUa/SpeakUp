namespace SpeakUp.Pages;

public partial class WorkflowListPage : ContentPage
{
    public WorkflowListPage(WorkflowListPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is WorkflowListPageViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
