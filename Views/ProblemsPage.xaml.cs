using PiClientV1.Services;
using PiServer.version_2.models;

namespace PiClientV1.Views
{
    public partial class ProblemsPage : ContentPage
    {
        private string _currentSessionId = string.Empty;
        private string _currentMode = "learning";
        private bool _isLearningActive = false;
        private List<string> _learningHistory = new List<string>();

        public ProblemsPage()
        {
            InitializeComponent();
            ModePicker.SelectedIndex = 0;
        }

        private void OnModeChanged(object sender, EventArgs e)
        {
            _currentMode = ModePicker.SelectedItem as string ?? "learning";
            ModeDescriptionLabel.Text = _currentMode == "learning"
                ? "Режим обучения: вы вводите каждый шаг вручную"
                : "Автоматический режим: система выполняет шаги автоматически";
        }

        private async void OnStartLearningClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProcessInput.Text))
            {
                await DisplayAlert("Ошибка", "Введите процесс π-исчисления", "OK");
                return;
            }

            try
            {
                var apiService = new ApiService();
                var response = await apiService.StartLearningSessionAsync(ProcessInput.Text, _currentMode);

                if (response != null)
                {
                    _currentSessionId = response.SessionId;
                    _isLearningActive = true;
                    _learningHistory.Clear();

                    // Показываем информацию о сессии
                    SessionInfoFrame.IsVisible = true;
                    SessionIdLabel.Text = $"Сессия: {response.SessionId}";
                    CurrentStateLabel.Text = $"Состояние: {response.CurrentState}";
                    HintLabel.Text = $"Подсказка: {response.Hint}";

                    // Настраиваем интерфейс в зависимости от режима
                    if (_currentMode == "learning")
                    {
                        StepInputFrame.IsVisible = true;
                        TaskDescriptionLabel.Text = response.Hint;
                    }

                    HistoryFrame.IsVisible = true;

                    // Добавляем в историю
                    _learningHistory.Add($"🎓 Начало обучения: {ProcessInput.Text}");
                    _learningHistory.Add($"📝 Режим: {response.Mode}");
                    _learningHistory.Add($"💡 {response.Hint}");
                    HistoryList.ItemsSource = new List<string>(_learningHistory);

                    await DisplayAlert("Успех", "Сессия обучения запущена", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось начать сессию обучения", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка запуска обучения: {ex.Message}", "OK");
            }
        }

        private async void OnSubmitStepClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StepInput.Text))
            {
                await DisplayAlert("Ошибка", "Введите ваш шаг", "OK");
                return;
            }

            try
            {
                var apiService = new ApiService();
                var result = await apiService.ExecuteLearningStepAsync(_currentSessionId, StepInput.Text);

                if (result != null)
                {
                    // ПОКАЗЫВАЕМ ВСЕ ПОЛЯ ИЗ LearningStepResult

                    // 1. Результат проверки
                    ResultFrame.IsVisible = true;
                    if (result.IsUserStepCorrect == true)
                    {
                        ResultLabel.Text = "✅ ПРАВИЛЬНО!";
                        ResultLabel.TextColor = Color.FromArgb("#4CAF50");
                    }
                    else
                    {
                        ResultLabel.Text = "❌ НЕПРАВИЛЬНО";
                        ResultLabel.TextColor = Color.FromArgb("#F44336");
                    }

                    FeedbackLabel.Text = result.Feedback ?? "Нет обратной связи";
                    ExplanationLabel.Text = result.Explanation ?? "Нет объяснения";

                    // 2. Основная информация из StepResult
                    StepInfoFrame.IsVisible = true;
                    StepCurrentStateLabel.Text = $"Состояние: {result.CurrentState}";
                    StepLastActionLabel.Text = $"Последнее действие: {result.LastAction}";

                    // 3. Параллельные действия
                    if (result.ParallelActions?.Any() == true)
                    {
                        ParallelActionsFrame.IsVisible = true;
                        ParallelActionsList.ItemsSource = result.ParallelActions;
                    }

                    // 4. Доступные редукции
                    if (result.AvailableReductions?.Any() == true)
                    {
                        ReductionsFrame.IsVisible = true;
                        ReductionsList.ItemsSource = result.AvailableReductions;
                    }

                    // 5. Обновляем основное состояние
                    CurrentStateLabel.Text = $"Состояние: {result.CurrentState}";
                    HintLabel.Text = $"Подсказка: {result.Hint ?? "Нет подсказки"}";

                    // 6. Добавляем в историю ВСЕ данные
                    _learningHistory.Add($"--- Шаг: {StepInput.Text} ---");
                    _learningHistory.Add($"✅ Правильно: {result.IsUserStepCorrect}");
                    _learningHistory.Add($"💬 Feedback: {result.Feedback}");
                    _learningHistory.Add($"📖 Explanation: {result.Explanation}");
                    _learningHistory.Add($"📊 State: {result.CurrentState}");
                    _learningHistory.Add($"📝 Action: {result.LastAction}");

                    if (result.AvailableReductions?.Any() == true)
                    {
                        _learningHistory.Add("🔄 Available Reductions:");
                        foreach (var reduction in result.AvailableReductions)
                            _learningHistory.Add($"   - {reduction}");
                    }

                    HistoryList.ItemsSource = new List<string>(_learningHistory);
                    StepInput.Text = string.Empty;

                    // Проверяем завершение
                    if (result.IsCompleted)
                    {
                        ResultLabel.Text = "🎉 ЗАДАЧА РЕШЕНА!";
                        ResultLabel.TextColor = Color.FromArgb("#4CAF50");
                        FeedbackLabel.Text = "Поздравляем! Вы успешно завершили процесс обучения.";

                        _learningHistory.Add("🎉 ЗАДАЧА УСПЕШНО РЕШЕНА!");
                        HistoryList.ItemsSource = new List<string>(_learningHistory);

                        StepInputFrame.IsVisible = false;
                        GetHintButton.IsEnabled = false;

                        await DisplayAlert("Поздравляем!", "Задача успешно решена!", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось проверить шаг", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка выполнения шага: {ex.Message}", "OK");
            }
        }

        private async void OnGetHintClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentSessionId))
            {
                await DisplayAlert("Ошибка", "Сначала начните сессию обучения", "OK");
                return;
            }

            try
            {
                var apiService = new ApiService();
                var hint = await apiService.GetLearningHintAsync(_currentSessionId);

                if (hint != null)
                {
                    TaskDescriptionLabel.Text = hint.Hint;
                    HintLabel.Text = $"Подсказка: {hint.Hint}";

                    _learningHistory.Add($"💡 Подсказка: {hint.Hint}");
                    HistoryList.ItemsSource = new List<string>(_learningHistory);

                    await DisplayAlert("Подсказка", hint.Hint, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка получения подсказки: {ex.Message}", "OK");
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            _currentSessionId = string.Empty;
            _isLearningActive = false;
            _learningHistory.Clear();

            // Сбрасываем интерфейс
            SessionInfoFrame.IsVisible = false;
            StepInputFrame.IsVisible = false;
            ResultFrame.IsVisible = false;
            StepInfoFrame.IsVisible = false;
            ParallelActionsFrame.IsVisible = false;
            ReductionsFrame.IsVisible = false;
            HistoryFrame.IsVisible = false;

            ProcessInput.Text = string.Empty;
            StepInput.Text = string.Empty;
        }

        private async void OnRefreshClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentSessionId)) return;

            try
            {
                var apiService = new ApiService();
                var status = await apiService.GetLearningStatusAsync(_currentSessionId);

                if (status != null)
                {
                    CurrentStateLabel.Text = $"Состояние: {status.CurrentState}";
                    HintLabel.Text = $"Подсказка: {status.Hint}";

                    _learningHistory.Add("🔄 Состояние обновлено");
                    HistoryList.ItemsSource = new List<string>(_learningHistory);

                    await DisplayAlert("Обновлено", "Статус обновлен", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка обновления: {ex.Message}", "OK");
            }
        }
    }
}