using PiClientV1.Models;
using PiClientV1.Services;

namespace PiClientV1.Views
{
    public partial class EmulatorPage : ContentPage
    {
        private string _currentSessionId = string.Empty;
        private bool _isProcessRunning = false;
        private int _stepCounter = 0;

        public EmulatorPage()
        {
            try
            {
                InitializeComponent();
                InitializeCollections();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания EmulatorPage: {ex}");
                throw;
            }
        }

        private void InitializeCollections()
        {
            ParallelActionsList.ItemsSource = new List<string> { "Нет активных действий" };
            HistoryList.ItemsSource = new List<string> { "История выполнения пуста" };
        }

        private async void OnStartProcessClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProcessInput.Text))
            {
                await DisplayAlert("Ошибка", "Введите процесс π-исчисления", "OK");
                return;
            }

            try
            {
                var processDefinition = ProcessInput.Text.Trim();

                var apiService = new ApiService();
                var response = await apiService.StartProcessAsync(processDefinition);

                if (response != null)
                {
                    _currentSessionId = response.SessionId;
                    _isProcessRunning = true;
                    _stepCounter = 0;

                    SessionInfoLabel.Text = $"Сессия: {_currentSessionId}";
                    CurrentStateLabel.Text = response.CurrentState;
                    LastActionLabel.Text = "Последнее действие: Процесс инициализирован";
                    CompletionLabel.Text = "Статус: 🟡 Выполняется";
                    StepButton.IsEnabled = true;
                    StartButton.IsEnabled = false;

                    await DisplayAlert("Успех", "Процесс запущен", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка запуска: {ex.Message}", "OK");
            }
        }

        private async void OnStepClicked(object sender, EventArgs e)
        {
            if (!_isProcessRunning || string.IsNullOrEmpty(_currentSessionId))
            {
                await DisplayAlert("Ошибка", "Сначала запустите процесс", "OK");
                return;
            }

            try
            {
                var apiService = new ApiService();
                var result = await apiService.ExecuteStepAsync(_currentSessionId);

                if (result != null)
                {
                    _stepCounter++;

                    CurrentStateLabel.Text = result.CurrentState;
                    LastActionLabel.Text = $"Последнее действие: {result.LastAction}";

                    if (result.ParallelActions?.Any() == true)
                    {
                        ParallelActionsList.ItemsSource = result.ParallelActions;
                    }

                    var history = new List<string> { $"[Шаг {_stepCounter}] {result.LastAction}" };
                    HistoryList.ItemsSource = history;

                    if (result.IsCompleted)
                    {
                        _isProcessRunning = false;
                        StepButton.IsEnabled = false;
                        CompletionLabel.Text = "Статус: ✅ Процесс завершен";
                        await DisplayAlert("Завершено", "Процесс успешно завершен", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка выполнения шага: {ex.Message}", "OK");
            }
        }

        private async void OnRefreshStateClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentSessionId))
            {
                await DisplayAlert("Ошибка", "Сначала запустите процесс", "OK");
                return;
            }

            try
            {
                var apiService = new ApiService();
                var state = await apiService.GetStateAsync(_currentSessionId);

                if (state != null)
                {
                    CurrentStateLabel.Text = state.CurrentState;
                    if (state.IsCompleted)
                    {
                        _isProcessRunning = false;
                        StepButton.IsEnabled = false;
                        CompletionLabel.Text = "Статус: ✅ Завершено";
                    }
                    await DisplayAlert("Обновлено", "Состояние процесса обновлено", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка обновления: {ex.Message}", "OK");
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            _currentSessionId = string.Empty;
            _isProcessRunning = false;
            _stepCounter = 0;

            ProcessInput.Text = string.Empty;
            SessionInfoLabel.Text = "Сессия не запущена";
            CurrentStateLabel.Text = "Введите процесс и нажмите 'Запустить'";
            LastActionLabel.Text = "Последнее действие: —";
            CompletionLabel.Text = "Статус: Ожидание запуска";

            StepButton.IsEnabled = false;
            StartButton.IsEnabled = true;

            InitializeCollections();
        }

        private async void OnExamplesClicked(object sender, EventArgs e)
        {
            var examples = new[]
            {
                "a![b].0 | a?(x).0",
                "x![y].P | x?(z).Q",
                "a?(x).(x![done].0 | b?(y).0)",
                "a![x].0 | a?(c).c![done].0"
            };

            var selected = await DisplayActionSheet("Выберите пример:", "Отмена", null, examples);

            if (selected != null && selected != "Отмена")
            {
                ProcessInput.Text = selected;
            }
        }
    }
}