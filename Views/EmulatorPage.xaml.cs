using PiClientV1.Services;

namespace PiClientV1.Views
{
    public partial class EmulatorPage : ContentPage
    {
        private string _currentSessionId = string.Empty;
        private bool _isProcessRunning = false;
        private int _stepCounter = 0;
        private List<string> _executionHistory = new List<string>();

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
            VariablesList.ItemsSource = new List<string> { "Нет переменных" };
            ChannelStatesList.ItemsSource = new List<string> { "Нет данных о каналах" };
            ActiveRestrictionsList.ItemsSource = new List<string> { "Нет активных ограничений" };
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
                    _executionHistory.Clear();

                    SessionInfoLabel.Text = $"Сессия: {_currentSessionId}";
                    CurrentStateLabel.Text = response.CurrentState;
                    LastActionLabel.Text = "Последнее действие: Процесс инициализирован";
                    CompletionLabel.Text = "Статус: 🟡 Выполняется";
                    StepButton.IsEnabled = true;
                    StartButton.IsEnabled = false;

                    // Добавляем в историю
                    _executionHistory.Add($"🚀 Процесс запущен: {processDefinition}");
                    _executionHistory.Add($"📁 Сессия: {response.SessionId}");
                    _executionHistory.Add($"📊 Начальное состояние: {response.CurrentState}");
                    HistoryList.ItemsSource = new List<string>(_executionHistory);

                    await DisplayAlert("Успех", "Процесс запущен", "OK");
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось запустить процесс", "OK");
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
                System.Diagnostics.Debug.WriteLine($"=== OnStepClicked: sessionId={_currentSessionId} ===");

                var apiService = new ApiService();
                var stepResult = await apiService.ExecuteStepAsync(_currentSessionId);

                System.Diagnostics.Debug.WriteLine($"=== StepResult received: {stepResult != null} ===");
                System.Diagnostics.Debug.WriteLine($"=== StepResult type: {stepResult?.GetType().Name} ===");

                if (stepResult != null)
                {
                    _stepCounter++;

                    // ОБНОВЛЯЕМ ВСЕ ПОЛЯ ИЗ StepResult
                    CurrentStateLabel.Text = stepResult.CurrentState;
                    LastActionLabel.Text = $"Последнее действие: {stepResult.LastAction}";

                    // Параллельные действия
                    if (stepResult.ParallelActions?.Any() == true)
                    {
                        ParallelActionsList.ItemsSource = stepResult.ParallelActions;
                    }
                    else
                    {
                        ParallelActionsList.ItemsSource = new List<string> { "Нет параллельных действий" };
                    }

                    // Переменные
                    if (stepResult.Variables?.Any() == true)
                    {
                        var variables = stepResult.Variables.Select(v => $"{v.Key} = {v.Value}").ToList();
                        VariablesList.ItemsSource = variables;
                    }
                    else
                    {
                        VariablesList.ItemsSource = new List<string> { "Нет переменных" };
                    }

                    // Состояния каналов
                    if (stepResult.ChannelStates?.Any() == true)
                    {
                        var channelStates = stepResult.ChannelStates.SelectMany(cs =>
                            cs.Value.Select(value => $"{cs.Key}: {value}")).ToList();
                        ChannelStatesList.ItemsSource = channelStates;
                    }
                    else
                    {
                        ChannelStatesList.ItemsSource = new List<string> { "Нет данных о каналах" };
                    }

                    // Активные ограничения
                    if (stepResult.ActiveRestrictions?.Any() == true)
                    {
                        ActiveRestrictionsList.ItemsSource = stepResult.ActiveRestrictions;
                    }
                    else
                    {
                        ActiveRestrictionsList.ItemsSource = new List<string> { "Нет активных ограничений" };
                    }

                    // Добавляем в историю ВСЕ данные
                    _executionHistory.Add($"--- Шаг {_stepCounter} ---");
                    _executionHistory.Add($"📝 Действие: {stepResult.LastAction}");
                    _executionHistory.Add($"📊 Состояние: {stepResult.CurrentState}");
                    _executionHistory.Add($"✅ Завершен: {stepResult.IsCompleted}");

                    if (stepResult.ParallelActions?.Any() == true)
                    {
                        _executionHistory.Add("🔄 Параллельные действия:");
                        foreach (var action in stepResult.ParallelActions)
                            _executionHistory.Add($"   - {action}");
                    }

                    if (stepResult.Variables?.Any() == true)
                    {
                        _executionHistory.Add("📝 Переменные:");
                        foreach (var variable in stepResult.Variables)
                            _executionHistory.Add($"   - {variable.Key} = {variable.Value}");
                    }

                    HistoryList.ItemsSource = new List<string>(_executionHistory);

                    if (stepResult.IsCompleted)
                    {
                        _isProcessRunning = false;
                        StepButton.IsEnabled = false;
                        CompletionLabel.Text = "Статус: ✅ Процесс завершен";
                        _executionHistory.Add("🎉 ПРОЦЕСС ЗАВЕРШЕН!");
                        HistoryList.ItemsSource = new List<string>(_executionHistory);
                        await DisplayAlert("Завершено", "Процесс успешно завершен", "OK");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"🔴 StepResult is NULL!");
                    await DisplayAlert("Ошибка", "Не удалось выполнить шаг (вернулся null)", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔴 OnStepClicked ERROR: {ex}");
                await DisplayAlert("Ошибка", $"Ошибка выполнения шага: {ex.Message}", "OK");

                // Добавляем информацию об ошибке в историю
                _executionHistory.Add($"❌ Ошибка на шаге {_stepCounter + 1}: {ex.Message}");
                HistoryList.ItemsSource = new List<string>(_executionHistory);
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            _currentSessionId = string.Empty;
            _isProcessRunning = false;
            _stepCounter = 0;
            _executionHistory.Clear();

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