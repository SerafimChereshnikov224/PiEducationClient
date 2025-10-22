namespace PiClientV1.Models;

public class ProcessRequest
{
    public string ProcessDefinition { get; set; } = string.Empty;
}

public class ProcessResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
}

public class ProcessState
{
    public string CurrentState { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}

public class LearningRequest
{
    public string ProcessDefinition { get; set; } = string.Empty;
    public string Mode { get; set; } = "learning";
}

public class LearningResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string CurrentState { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public string? ExpectedNextStep { get; set; }
}

public class StepVerificationRequest
{
    public string UserInput { get; set; } = string.Empty;
}

public class StepResult
{
    public string CurrentState { get; set; } = string.Empty;
    public string LastAction { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public List<string> ParallelActions { get; set; } = new();
    public bool? IsCorrect { get; set; }
    public string? Feedback { get; set; }
    public string? ExpectedStep { get; set; }
}

public class LearningStatus
{
    public bool RequiresInput { get; set; }
    public bool IsCompleted { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public string ExpectedNextStep { get; set; } = string.Empty;
}

public class HintResponse
{
    public string Hint { get; set; } = string.Empty;
    public string ExpectedNextStep { get; set; } = string.Empty;
}

public class StepDescription
{
    public string Description { get; set; } = string.Empty;
    public string ExpectedExpression { get; set; } = string.Empty;
}