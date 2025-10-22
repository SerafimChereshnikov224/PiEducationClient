using System.Net.Http.Json;
using PiClientV1.Models;

namespace PiClientV1.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://localhost:7000/");

        // Для разработки - отключаем SSL проверку
#if DEBUG
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _httpClient = new HttpClient(handler);
        _httpClient.BaseAddress = new Uri("https://localhost:7000/");
#endif
    }

    // Базовые методы
    public async Task<ProcessResponse?> StartProcessAsync(string processDefinition)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: POST /api/pi/start
        // var request = new ProcessRequest { ProcessDefinition = processDefinition };
        // var response = await _httpClient.PostAsJsonAsync("api/pi/start", request);
        // return await response.Content.ReadFromJsonAsync<ProcessResponse>();

        await Task.Delay(500);
        return new ProcessResponse
        {
            SessionId = Guid.NewGuid().ToString(),
            CurrentState = processDefinition
        };
    }

    public async Task<StepResult?> ExecuteStepAsync(string sessionId)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: POST /api/pi/{sessionId}/step
        // var response = await _httpClient.PostAsync($"api/pi/{sessionId}/step", null);
        // return await response.Content.ReadFromJsonAsync<StepResult>();

        await Task.Delay(300);
        return new StepResult
        {
            CurrentState = "0 | 0",
            LastAction = "Выполнена коммуникация по каналу 'a'",
            IsCompleted = true,
            ParallelActions = new List<string> { "Отправка сообщения", "Получение сообщения" }
        };
    }

    // Методы обучения
    public async Task<LearningResponse?> StartLearningSessionAsync(string processDefinition, string mode = "learning")
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: POST /api/pi/learning/start
        // var request = new LearningRequest { ProcessDefinition = processDefinition, Mode = mode };
        // var response = await _httpClient.PostAsJsonAsync("api/pi/learning/start", request);
        // return await response.Content.ReadFromJsonAsync<LearningResponse>();

        await Task.Delay(500);
        return new LearningResponse
        {
            SessionId = "learn-" + Guid.NewGuid().ToString(),
            CurrentState = processDefinition,
            Mode = mode,
            Hint = mode == "learning"
                ? "Введите следующий шаг вычисления"
                : "Автоматический режим выполнения",
            ExpectedNextStep = "0 | 0"
        };
    }

    public async Task<StepResult?> ExecuteLearningStepAsync(string sessionId, string userInput)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: POST /api/pi/{sessionId}/learning/step
        // var request = new StepVerificationRequest { UserInput = userInput };
        // var response = await _httpClient.PostAsJsonAsync($"api/pi/{sessionId}/learning/step", request);
        // return await response.Content.ReadFromJsonAsync<StepResult>();

        await Task.Delay(400);
        bool isCorrect = userInput.Trim() == "0 | 0";

        return new StepResult
        {
            CurrentState = "0 | 0",
            LastAction = "Коммуникация завершена",
            IsCompleted = true,
            IsCorrect = isCorrect,
            Feedback = isCorrect
                ? "Правильно! Процесс завершен."
                : $"Неправильно. Ожидалось: 0 | 0",
            ExpectedStep = "0 | 0",
            ParallelActions = new List<string> { "Проверка шага пользователя" }
        };
    }

    public async Task<StepResult?> ExecuteAutoStepAsync(string sessionId)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: POST /api/pi/{sessionId}/learning/auto-step
        // var response = await _httpClient.PostAsync($"api/pi/{sessionId}/learning/auto-step", null);
        // return await response.Content.ReadFromJsonAsync<StepResult>();

        await Task.Delay(300);
        return new StepResult
        {
            CurrentState = "0 | 0",
            LastAction = "Автоматически выполнена коммуникация",
            IsCompleted = true,
            ParallelActions = new List<string> { "Авто-выполнение шага" }
        };
    }

    public async Task<ProcessState?> GetStateAsync(string sessionId)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: GET /api/pi/{sessionId}
        // var response = await _httpClient.GetAsync($"api/pi/{sessionId}");
        // return await response.Content.ReadFromJsonAsync<ProcessState>();

        await Task.Delay(200);
        return new ProcessState
        {
            CurrentState = sessionId.Contains("learn")
                ? "a![b].0 | a?(x).0"
                : "0 | 0",
            IsCompleted = sessionId.Contains("learn") ? false : true
        };
    }

    // Дополнительные методы-заглушки для полного покрытия API
    public async Task<LearningStatus?> GetLearningStatusAsync(string sessionId)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: GET /api/pi/{sessionId}/learning/status
        // var response = await _httpClient.GetAsync($"api/pi/{sessionId}/learning/status");
        // return await response.Content.ReadFromJsonAsync<LearningStatus>();

        await Task.Delay(200);
        return new LearningStatus
        {
            RequiresInput = true,
            IsCompleted = false,
            CurrentState = "a![b].0 | a?(x).0",
            Hint = "Введите следующий шаг после коммуникации",
            ExpectedNextStep = "0 | 0"
        };
    }

    public async Task<HintResponse?> GetHintAsync(string sessionId)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: GET /api/pi/{sessionId}/learning/hint
        // var response = await _httpClient.GetAsync($"api/pi/{sessionId}/learning/hint");
        // return await response.Content.ReadFromJsonAsync<HintResponse>();

        await Task.Delay(200);
        return new HintResponse
        {
            Hint = "После коммуникации процессы завершаются",
            ExpectedNextStep = "0 | 0"
        };
    }

    // Дополнительные методы из вашего API
    public async Task<StepDescription?> GetStepDescriptionAsync(string sessionId)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: GET /api/pi/{sessionId}/learning/description
        // var response = await _httpClient.GetAsync($"api/pi/{sessionId}/learning/description");
        // return await response.Content.ReadFromJsonAsync<StepDescription>();

        await Task.Delay(200);
        return new StepDescription
        {
            Description = "Коммуникация между процессами",
            ExpectedExpression = "0 | 0"
        };
    }

    public async Task<object?> SwitchLearningModeAsync(string sessionId, string mode)
    {
        // РЕАЛЬНЫЙ ВЫЗОВ: POST /api/pi/{sessionId}/learning/switch-mode
        // var response = await _httpClient.PostAsJsonAsync($"api/pi/{sessionId}/learning/switch-mode", mode);
        // return await response.Content.ReadFromJsonAsync<object>();

        await Task.Delay(200);
        return new { Message = "Режим изменен", CurrentMode = mode };
    }
}