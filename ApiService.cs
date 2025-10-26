using System.Net.Http.Json;
using PiClientV1.Models;

namespace PiClientV1.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== ApiService constructor started ===");

            // Упрощаем создание HttpClient - убираем HTTPS для тестирования
            _httpClient = new HttpClient();

            // Меняем на HTTP для тестирования или убираем BaseAddress
            _httpClient.BaseAddress = new Uri("http://localhost:7000/");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            System.Diagnostics.Debug.WriteLine("=== ApiService created successfully ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 ApiService constructor ERROR: {ex}");
            throw;
        }
    }

    public async Task<ProcessResponse?> StartProcessAsync(string processDefinition, CancellationToken ct = default)
    {
        System.Diagnostics.Debug.WriteLine($"=== StartProcessAsync called: {processDefinition} ===");

        try
        {
            // ЗАГЛУШКА: Сначала просто возвращаем тестовые данные без реального HTTP
            await Task.Delay(500, ct);

            System.Diagnostics.Debug.WriteLine("=== StartProcessAsync returning stub data ===");

            return new ProcessResponse
            {
                SessionId = Guid.NewGuid().ToString(),
                CurrentState = processDefinition
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 StartProcessAsync ERROR: {ex}");
            return null;
        }
    }

    public async Task<StepResult?> ExecuteStepAsync(string sessionId, CancellationToken ct = default)
    {
        System.Diagnostics.Debug.WriteLine($"=== ExecuteStepAsync called: {sessionId} ===");

        try
        {
            // ЗАГЛУШКА
            await Task.Delay(300, ct);

            System.Diagnostics.Debug.WriteLine("=== ExecuteStepAsync returning stub data ===");

            return new StepResult
            {
                CurrentState = "0 | 0",
                LastAction = "Выполнена коммуникация по каналу 'a'",
                IsCompleted = true,
                ParallelActions = new List<string> { "Отправка сообщения", "Получение сообщения" }
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 ExecuteStepAsync ERROR: {ex}");
            return null;
        }
    }

    public async Task<ProcessState?> GetStateAsync(string sessionId, CancellationToken ct = default)
    {
        System.Diagnostics.Debug.WriteLine($"=== GetStateAsync called: {sessionId} ===");

        try
        {
            // ЗАГЛУШКА
            await Task.Delay(200, ct);

            return new ProcessState
            {
                CurrentState = "0 | 0",
                IsCompleted = true
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 GetStateAsync ERROR: {ex}");
            return null;
        }
    }
}