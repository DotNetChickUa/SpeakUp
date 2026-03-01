namespace SpeakUp.Pages;

public partial class CommandHistoryPage : ContentPage
{
    public CommandHistoryPage(CommandHistoryPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is CommandHistoryPageViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
