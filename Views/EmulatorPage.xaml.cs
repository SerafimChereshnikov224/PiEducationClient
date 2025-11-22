using PiClientV1.Services;
using System.Collections.ObjectModel;

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
            ParallelActionsList.ItemsSource = new ObservableCollection<string> { "Нет активных действий" };
            VariablesList.ItemsSource = new ObservableCollection<string> { "Нет переменных" };
            ChannelStatesList.ItemsSource = new ObservableCollection<string> { "Нет данных о каналах" };
            ActiveRestrictionsList.ItemsSource = new ObservableCollection<string> { "Нет активных ограничений" };
            HistoryList.ItemsSource = new ObservableCollection<string> { "История выполнения пуста" };
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
                    HistoryList.ItemsSource = new ObservableCollection<string>(_executionHistory);

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

                if (stepResult != null)
                {
                    _stepCounter++;

                    // ОБНОВЛЯЕМ ВСЕ ПОЛЯ ИЗ StepResult
                    CurrentStateLabel.Text = stepResult.CurrentState ?? "Не указано";
                    LastActionLabel.Text = $"Последнее действие: {stepResult.LastAction ?? "Не указано"}";

                    // ДЕБАГ ИНФОРМАЦИЯ
                    System.Diagnostics.Debug.WriteLine($"=== StepResult Data ===");
                    System.Diagnostics.Debug.WriteLine($"CurrentState: {stepResult.CurrentState}");
                    System.Diagnostics.Debug.WriteLine($"LastAction: {stepResult.LastAction}");
                    System.Diagnostics.Debug.WriteLine($"IsCompleted: {stepResult.IsCompleted}");
                    System.Diagnostics.Debug.WriteLine($"ParallelActions count: {stepResult.ParallelActions?.Count ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"Variables count: {stepResult.Variables?.Count ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"ChannelStates count: {stepResult.ChannelStates?.Count ?? 0}");
                    System.Diagnostics.Debug.WriteLine($"ActiveRestrictions count: {stepResult.ActiveRestrictions?.Count ?? 0}");

                    // Параллельные действия - используем ObservableCollection для обновления UI
                    var parallelActions = new ObservableCollection<string>();
                    if (stepResult.ParallelActions?.Any() == true)
                    {
                        foreach (var action in stepResult.ParallelActions)
                        {
                            parallelActions.Add(action);
                        }
                    }
                    else
                    {
                        parallelActions.Add("Нет параллельных действий");
                    }
                    ParallelActionsList.ItemsSource = parallelActions;

                    // Переменные - используем ObservableCollection
                    var variables = new ObservableCollection<string>();
                    if (stepResult.Variables?.Any() == true)
                    {
                        foreach (var variable in stepResult.Variables)
                        {
                            variables.Add($"{variable.Key} = {variable.Value}");
                        }
                    }
                    else
                    {
                        variables.Add("Нет переменных");
                    }
                    VariablesList.ItemsSource = variables;

                    // Состояния каналов - используем ObservableCollection
                    var channelStates = new ObservableCollection<string>();
                    if (stepResult.ChannelStates?.Any() == true)
                    {
                        foreach (var channel in stepResult.ChannelStates)
                        {
                            if (channel.Value?.Any() == true)
                            {
                                foreach (var value in channel.Value)
                                {
                                    channelStates.Add($"{channel.Key}: {value}");
                                }
                            }
                            else
                            {
                                channelStates.Add($"{channel.Key}: нет данных");
                            }
                        }
                    }
                    else
                    {
                        channelStates.Add("Нет данных о каналах");
                    }
                    ChannelStatesList.ItemsSource = channelStates;

                    // Активные ограничения - используем ObservableCollection
                    var activeRestrictions = new ObservableCollection<string>();
                    if (stepResult.ActiveRestrictions?.Any() == true)
                    {
                        foreach (var restriction in stepResult.ActiveRestrictions)
                        {
                            activeRestrictions.Add(restriction);
                        }
                    }
                    else
                    {
                        activeRestrictions.Add("Нет активных ограничений");
                    }
                    ActiveRestrictionsList.ItemsSource = activeRestrictions;

                    // Добавляем в историю ВСЕ данные
                    _executionHistory.Add($"============== ШАГ {_stepCounter} ==============");
                    _executionHistory.Add($"📝 Действие: {stepResult.LastAction ?? "Не указано"}");
                    _executionHistory.Add($"📊 Состояние: {stepResult.CurrentState ?? "Не указано"}");
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

                    if (stepResult.ChannelStates?.Any() == true)
                    {
                        _executionHistory.Add("📡 Состояния каналов:");
                        foreach (var channel in stepResult.ChannelStates)
                        {
                            if (channel.Value?.Any() == true)
                            {
                                foreach (var value in channel.Value)
                                    _executionHistory.Add($"   - {channel.Key}: {value}");
                            }
                            else
                            {
                                _executionHistory.Add($"   - {channel.Key}: нет данных");
                            }
                        }
                    }

                    if (stepResult.ActiveRestrictions?.Any() == true)
                    {
                        _executionHistory.Add("🚫 Активные ограничения:");
                        foreach (var restriction in stepResult.ActiveRestrictions)
                            _executionHistory.Add($"   - {restriction}");
                    }

                    // Используем ObservableCollection для истории
                    HistoryList.ItemsSource = new ObservableCollection<string>(_executionHistory);

                    // Прокручиваем историю к последнему элементу
                    if (HistoryList.ItemsSource is ObservableCollection<string> historyCollection && historyCollection.Any())
                    {
                        var lastItem = historyCollection.Last();
                        HistoryList.ScrollTo(lastItem, ScrollToPosition.End, false);
                    }

                    if (stepResult.IsCompleted)
                    {
                        _isProcessRunning = false;
                        StepButton.IsEnabled = false;
                        CompletionLabel.Text = "Статус: ✅ Процесс завершен";
                        _executionHistory.Add("🎉 ПРОЦЕСС ЗАВЕРШЕН!");
                        HistoryList.ItemsSource = new ObservableCollection<string>(_executionHistory);
                        await DisplayAlert("Завершено", "Процесс успешно завершен", "OK");
                    }
                    else
                    {
                        CompletionLabel.Text = "Статус: 🟡 Выполняется";
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
                HistoryList.ItemsSource = new ObservableCollection<string>(_executionHistory);
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