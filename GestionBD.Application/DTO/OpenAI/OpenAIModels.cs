namespace GestionBD.Application.DTO.OpenAI;

// Request DTOs
public sealed record OpenAIValidationRequest(
    string Model,
    List<OpenAIMessage> Input,
    OpenAITextFormat Text,
    OpenAIReasoning Reasoning,
    int MaxOutputTokens
);

public sealed record OpenAIMessage(
    string Role,
    List<OpenAIContent> Content
);

public sealed record OpenAIContent(
    string Type,
    string Text
);

public sealed record OpenAITextFormat(
    OpenAIFormatType Format
);

public sealed record OpenAIFormatType(
    string Type
);

public sealed record OpenAIReasoning(
    string Effort
);

// Response DTOs
public sealed record OpenAIValidationResponse(
    string Id,
    string Object,
    long CreatedAt,
    string Status,
    List<OpenAIOutput> Output,
    string Model,
    int MaxOutputTokens,
    OpenAIUsage Usage
);

public sealed record OpenAIOutput(
    string Id,
    string Type,
    string? Status,
    List<OpenAIContentOutput>? Content,
    string? Role
);

public sealed record OpenAIContentOutput(
    string Type,
    string Text
);

public sealed record OpenAIUsage(
    int InputTokens,
    int OutputTokens,
    int TotalTokens
);