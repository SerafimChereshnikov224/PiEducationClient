using PiServer.version_2.controllers;
using PiServer.version_2.models;
using LearningRequest = PiServer.version_2.controllers.LearningRequest;
using ProcessRequest = PiServer.version_2.controllers.ProcessRequest;
using ProcessResponse = PiServer.version_2.controllers.ProcessResponse;
using ProcessState = PiServer.version_2.controllers.ProcessState;
using StepVerificationRequest = PiServer.version_2.controllers.StepVerificationRequest;
using StepResult = PiServer.version_2.runtime.StepResult;
using PiServer.version_2.controllers;
using PiServer.version_2.models;

namespace PiClientV1.Services;

public class ApiService
{
    private readonly PiProcessApi _piProcessApi;

    public ApiService()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("=== ApiService constructor started ===");
            _piProcessApi = new PiProcessApi();
            System.Diagnostics.Debug.WriteLine("=== ApiService created successfully ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 ApiService constructor ERROR: {ex}");
            throw;
        }
    }

    // === МЕТОДЫ ДЛЯ ЭМУЛЯТОРА ===
    public async Task<ProcessResponse?> StartProcessAsync(string processDefinition, CancellationToken ct = default)
    {
        System.Diagnostics.Debug.WriteLine($"=== StartProcessAsync called: {processDefinition} ===");

        try
        {
            var request = new ProcessRequest
            {
                ProcessDefinition = processDefinition
            };

            var response = _piProcessApi.StartProcess(request);
            System.Diagnostics.Debug.WriteLine("=== StartProcessAsync returning real data ===");
            return response;
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
            var stepOutput = await _piProcessApi.ExecuteStepAsync(sessionId);

            // ПРЯМОЕ ПРИВЕДЕНИЕ ТИПА вместо as
            if (stepOutput is StepResult stepResult)
            {
                return stepResult;
            }

            // Если тип не StepResult, логируем и возвращаем null
            System.Diagnostics.Debug.WriteLine($"🔴 ExecuteStepAsync: Unexpected type: {stepOutput?.GetType().Name}");
            return null;
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
            var state = _piProcessApi.GetState(sessionId);
            return state;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 GetStateAsync ERROR: {ex}");
            return null;
        }
    }

    // === МЕТОДЫ ДЛЯ ОБУЧЕНИЯ ===
    public async Task<LearningStartResponse?> StartLearningSessionAsync(string processDefinition, string mode = "learning")
    {
        try
        {
            var request = new LearningRequest
            {
                ProcessDefinition = processDefinition,
                Mode = mode
            };

            return _piProcessApi.StartLearningSession(request);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 StartLearningSessionAsync ERROR: {ex}");
            return null;
        }
    }

    public async Task<LearningStepResult?> ExecuteLearningStepAsync(string sessionId, string userInput)
    {
        try
        {
            var request = new StepVerificationRequest
            {
                UserInput = userInput
            };

            var result = await _piProcessApi.ExecuteLearningStepAsync(sessionId, request);

            // ПРЯМОЕ ПРИВЕДЕНИЕ ТИПА вместо as
            if (result is LearningStepResult learningStepResult)
            {
                return learningStepResult;
            }

            System.Diagnostics.Debug.WriteLine($"🔴 ExecuteLearningStepAsync: Unexpected type: {result?.GetType().Name}");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 ExecuteLearningStepAsync ERROR: {ex}");
            return null;
        }
    }

    public async Task<LearningHintResponse?> GetLearningHintAsync(string sessionId)
    {
        try
        {
            return _piProcessApi.GetLearningHint(sessionId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 GetLearningHintAsync ERROR: {ex}");
            return null;
        }
    }

    public async Task<LearningStatusResponse?> GetLearningStatusAsync(string sessionId)
    {
        try
        {
            return _piProcessApi.GetLearningStatus(sessionId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"🔴 GetLearningStatusAsync ERROR: {ex}");
            return null;
        }
    }
}