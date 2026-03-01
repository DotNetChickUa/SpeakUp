namespace SpeakUp.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is SettingsPageViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
