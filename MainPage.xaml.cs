using PiClientV1.Services;

namespace PiClientV1;

public partial class MainPage : ContentPage
{
    private readonly ApiService _apiService;

    public MainPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private void OnExampleClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            ProcessEditor.Text = button.Text;
        }
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProcessEditor.Text))
        {
            ShowError("Введите определение процесса");
            return;
        }

        StartButton.IsEnabled = false;
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        ErrorLabel.IsVisible = false;

        try
        {
            var processDefinition = ProcessEditor.Text.Trim();
            var mode = LearningRadio.IsChecked ? "learning" : "auto";

            var response = await _apiService.StartLearningSessionAsync(processDefinition, mode);

            if (response != null)
            {
                // Переходим на страницу выполнения
                await Navigation.PushAsync(new ExecutionPage(response.SessionId, response.Mode));
            }
            else
            {
                ShowError("Не удалось создать сессию.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Ошибка: {ex.Message}");
        }
        finally
        {
            StartButton.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}