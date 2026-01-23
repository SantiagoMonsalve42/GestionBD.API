namespace GestionBD.Domain.ValueObjects;

public sealed class SqlValidation
{
    public bool IsValid { get; }
    public IReadOnlyList<ValidationError> Errors { get; }
    public IReadOnlyList<ValidationWarning> Warnings { get; }
    public IReadOnlyList<ValidationSuggestion> Suggestions { get; }

    private SqlValidation(
        bool isValid,
        IReadOnlyList<ValidationError> errors,
        IReadOnlyList<ValidationWarning> warnings,
        IReadOnlyList<ValidationSuggestion> suggestions)
    {
        IsValid = isValid;
        Errors = errors ?? [];
        Warnings = warnings ?? [];
        Suggestions = suggestions ?? [];
    }

    public static SqlValidation Create(
        bool isValid,
        IEnumerable<ValidationError> errors,
        IEnumerable<ValidationWarning> warnings,
        IEnumerable<ValidationSuggestion> suggestions)
    {
        return new SqlValidation(
            isValid,
            errors?.ToList().AsReadOnly() ?? new List<ValidationError>().AsReadOnly(),
            warnings?.ToList().AsReadOnly() ?? new List<ValidationWarning>().AsReadOnly(),
            suggestions?.ToList().AsReadOnly() ?? new List<ValidationSuggestion>().AsReadOnly()
        );
    }

    public static SqlValidation Valid() => 
        new(true, [], [], []);

    public static SqlValidation Invalid(params ValidationError[] errors) => 
        new(false, errors.ToList().AsReadOnly(), [], []);

    public bool HasErrors => Errors.Any();
    public bool HasWarnings => Warnings.Any();
    public bool HasSuggestions => Suggestions.Any();
}

public sealed record ValidationError(string Code, string Message);
public sealed record ValidationWarning(string Code, string Message);
public sealed record ValidationSuggestion(string Code, string Message);