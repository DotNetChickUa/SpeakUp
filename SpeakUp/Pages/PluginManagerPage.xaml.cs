namespace SpeakUp.Pages;

public partial class PluginManagerPage : ContentPage
{
    public PluginManagerPage(PluginManagerPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is PluginManagerPageViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}
