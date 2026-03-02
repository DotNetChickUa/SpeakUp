using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakUp.Models;
using SpeakUp.Services;
using System.Collections.ObjectModel;

namespace SpeakUp.Pages;

public sealed partial class WorkflowListPageViewModel : ObservableObject
{
    private readonly IWorkflowService _workflowService;
    private readonly IWorkflowExecutionService _executionService;
    private readonly IErrorHandlingService _errorHandler;

    public ObservableCollection<Workflow> Workflows { get; } = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "All";

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private int _totalWorkflows;

    [ObservableProperty]
    private int _enabledWorkflows;

    public ObservableCollection<string> Categories { get; } = new()
    {
        "All",
        "General",
        "Communication",
        "Automation",
        "System",
        "Testing",
        "Custom"
    };

    public WorkflowListPageViewModel(
        IWorkflowService workflowService,
        IWorkflowExecutionService executionService,
        IErrorHandlingService errorHandler)
    {
        _workflowService = workflowService;
        _executionService = executionService;
        _errorHandler = errorHandler;
    }

    public async Task InitializeAsync()
    {
        await _workflowService.InitializeAsync();
        await LoadWorkflowsAsync();
    }

    [RelayCommand]
    private async Task LoadWorkflowsAsync()
    {
        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;

            var workflows = SelectedCategory == "All"
                ? await _workflowService.GetAllWorkflowsAsync()
                : await _workflowService.GetWorkflowsByCategoryAsync(SelectedCategory);

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchResults = await _workflowService.SearchWorkflowsAsync(SearchText);
                workflows = workflows.Intersect(searchResults).ToList();
            }

            Workflows.Clear();
            foreach (var workflow in workflows)
            {
                Workflows.Add(workflow);
            }

            UpdateStatistics();
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Loading workflows");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CreateWorkflowAsync()
    {
        var name = await Shell.Current.DisplayPromptAsync(
            "New Workflow",
            "Enter workflow name:",
            "Create",
            "Cancel");

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        try
        {
            var workflow = new Workflow
            {
                Name = name,
                Category = SelectedCategory == "All" ? "General" : SelectedCategory,
                IsEnabled = true
            };

            await _workflowService.SaveWorkflowAsync(workflow);
            await LoadWorkflowsAsync();

            await Shell.Current.DisplayAlert("Success", $"Workflow '{name}' created", "OK");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, "Creating workflow");
        }
    }

    [RelayCommand]
    private async Task ExecuteWorkflowAsync(Workflow workflow)
    {
        if (workflow == null)
        {
            return;
        }

        try
        {
            var result = await _executionService.ExecuteWorkflowAsync(workflow.Id);

            var message = result.Success
                ? $"Workflow completed successfully!\n\nDuration: {result.Duration.TotalSeconds:F2}s\nSteps: {result.StepsExecuted}"
                : $"Workflow failed!\n\n{result.ErrorMessage}\n\nSteps completed: {result.StepsExecuted}";

            await Shell.Current.DisplayAlert(
                result.Success ? "✓ Success" : "✗ Failed",
                message,
                "OK");

            await LoadWorkflowsAsync();
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, $"Executing workflow '{workflow.Name}'");
        }
    }

    [RelayCommand]
    private async Task EditWorkflowAsync(Workflow workflow)
    {
        if (workflow == null)
        {
            return;
        }

        // Navigate to workflow editor
        await Shell.Current.GoToAsync($"WorkflowEditor?workflowId={workflow.Id}");
    }

    [RelayCommand]
    private async Task DeleteWorkflowAsync(Workflow workflow)
    {
        if (workflow == null)
        {
            return;
        }

        var confirm = await _errorHandler.ConfirmDestructiveActionAsync(
            $"delete workflow '{workflow.Name}'",
            "All steps and triggers will be permanently removed.");

        if (!confirm)
        {
            return;
        }

        try
        {
            await _workflowService.DeleteWorkflowAsync(workflow.Id);
            Workflows.Remove(workflow);
            UpdateStatistics();

            await Shell.Current.DisplayAlert("Success", $"Workflow '{workflow.Name}' deleted", "OK");
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, $"Deleting workflow '{workflow.Name}'");
        }
    }

    [RelayCommand]
    private async Task ToggleWorkflowAsync(Workflow workflow)
    {
        if (workflow == null)
        {
            return;
        }

        try
        {
            workflow.IsEnabled = !workflow.IsEnabled;
            await _workflowService.SaveWorkflowAsync(workflow);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, $"Toggling workflow '{workflow.Name}'");
        }
    }

    [RelayCommand]
    private async Task ValidateWorkflowAsync(Workflow workflow)
    {
        if (workflow == null)
        {
            return;
        }

        try
        {
            var (isValid, errors) = await _executionService.ValidateWorkflowAsync(workflow.Id);

            if (isValid)
            {
                await Shell.Current.DisplayAlert("✓ Valid", $"Workflow '{workflow.Name}' is valid", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "✗ Invalid",
                    $"Workflow '{workflow.Name}' has errors:\n\n{string.Join("\n", errors)}",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            await _errorHandler.HandleExceptionAsync(ex, $"Validating workflow '{workflow.Name}'");
        }
    }

    [RelayCommand]
    private static async Task NavigateBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void UpdateStatistics()
    {
        TotalWorkflows = Workflows.Count;
        EnabledWorkflows = Workflows.Count(w => w.IsEnabled);
    }

    partial void OnSearchTextChanged(string value)
    {
        _ = LoadWorkflowsAsync();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        _ = LoadWorkflowsAsync();
    }
}
