using PiClientV1.Models;
using PiClientV1.Services;

namespace PiClientV1;

public partial class ExecutionPage : ContentPage
{
    private readonly ApiService _apiService;
    private string _sessionId = string.Empty;
    private string _mode = string.Empty;

    public ExecutionPage(string sessionId, string mode)
    {
        InitializeComponent();
        _apiService = new ApiService();
        _sessionId = sessionId;
        _mode = mode;

        SessionLabel.Text = $"Сессия: {sessionId}";

        // Настраиваем интерфейс в зависимости от режима
        if (mode == "learning")
        {
            LearningPanel.IsVisible = true;
            Title = "Обучение - Выполнение процесса";
        }
        else
        {
            AutoStepButton.IsVisible = true;
            Title = "Авто - Выполнение процесса";
        }

        LoadCurrentState();
    }

    private async void LoadCurrentState()
    {
        var state = await _apiService.GetStateAsync(_sessionId);
        if (state != null)
        {
            StateLabel.Text = state.CurrentState;
            StatusLabel.Text = state.IsCompleted ? "Завершено" : "Выполняется";
        }
    }

    private async void OnAutoStepClicked(object sender, EventArgs e)
    {
        var result = await _apiService.ExecuteStepAsync(_sessionId);
        if (result != null)
        {
            UpdateUI(result);
        }
    }

    private async void OnVerifyStepClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(UserInputEntry.Text))
        {
            await DisplayAlert("Ошибка", "Введите шаг", "OK");
            return;
        }

        var result = await _apiService.ExecuteLearningStepAsync(_sessionId, UserInputEntry.Text);
        if (result != null)
        {
            UpdateUI(result);
            UserInputEntry.Text = string.Empty;
        }
    }

    private async void OnGetHintClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Подсказка", "Функция подсказки будет добавлена позже", "OK");
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        LoadCurrentState();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void UpdateUI(StepResult result)
    {
        StateLabel.Text = result.CurrentState;
        ActionLabel.Text = result.LastAction;
        StatusLabel.Text = result.IsCompleted ? "Завершено" : "Выполняется";

        if (result.IsCompleted)
        {
            AutoStepButton.IsEnabled = false;
            LearningPanel.IsVisible = false;
        }
    }
}