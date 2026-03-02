namespace SpeakUp.Pages;

[QueryProperty(nameof(WorkflowId), "workflowId")]
public partial class WorkflowEditorPage : ContentPage
{
    private int _workflowId;

    public int WorkflowId
    {
        get => _workflowId;
        set
        {
            _workflowId = value;
            if (BindingContext is WorkflowEditorPageViewModel viewModel)
            {
                viewModel.WorkflowId = value;
            }
        }
    }

    public WorkflowEditorPage(WorkflowEditorPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is WorkflowEditorPageViewModel viewModel && viewModel.WorkflowId > 0)
        {
            await viewModel.InitializeAsync(viewModel.WorkflowId);
        }
    }
}
