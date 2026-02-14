namespace GestionBD.Domain.ValueObjects;

public sealed class RollbackGeneration
{
    public IReadOnlyList<RollbackScript> Scripts { get; }
    public IReadOnlyList<string> Warnings { get; }
    public IReadOnlyList<string> Assumptions { get; }

    private RollbackGeneration(
        IReadOnlyList<RollbackScript> scripts,
        IReadOnlyList<string> warnings,
        IReadOnlyList<string> assumptions)
    {
        Scripts = scripts;
        Warnings = warnings;
        Assumptions = assumptions;
    }

    public static RollbackGeneration Create(
        IEnumerable<RollbackScript> scripts,
        IEnumerable<string> warnings,
        IEnumerable<string> assumptions)
    {
        return new RollbackGeneration(
            scripts?.ToList().AsReadOnly() ?? new List<RollbackScript>().AsReadOnly(),
            warnings?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly(),
            assumptions?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly()
        );
    }

    public bool HasWarnings => Warnings.Any();
    public bool HasAssumptions => Assumptions.Any();
    public int ScriptCount => Scripts.Count;
}

public sealed record RollbackScript(
    string FileName,
    string ObjectType,
    string ObjectName,
    int RollbackOrder,
    string Script,
    IReadOnlyList<string> DependsOn
);