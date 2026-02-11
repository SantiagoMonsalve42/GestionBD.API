namespace GestionBD.Application.DTO.OpenAI;

// Request DTOs
public sealed record RollbackGenerationRequest(
    string Model,
    List<OpenAIMessage> Input,
    OpenAITextFormat Text,
    OpenAIReasoning Reasoning,
    int MaxOutputTokens
);

// Response DTOs
public sealed record RollbackGenerationResponse(
    List<RollbackScript> RollbackScripts,
    List<string> Warnings,
    List<string> Assumptions
);



public sealed record RollbackScript(
    string FileName,
    string ObjectType,
    string ObjectName,
    int RollbackOrder,
    string Script,
    List<string> DependsOn
);