using GestionBD.Domain.ValueObjects;
using Xunit;

namespace GestionBD.UnitTests.Domain.ValueObjects;

public sealed class RollbackGenerationTests
{
    [Fact]
    public void Create_WithNullCollections_ShouldDefaultToEmptyLists()
    {
        IEnumerable<RollbackScript>? scripts = null;
        IEnumerable<string>? warnings = null;
        IEnumerable<string>? assumptions = null;

        var result = RollbackGeneration.Create(scripts, warnings, assumptions);

        Assert.Empty(result.Scripts);
        Assert.Empty(result.Warnings);
        Assert.Empty(result.Assumptions);
        Assert.False(result.HasWarnings);
        Assert.False(result.HasAssumptions);
        Assert.Equal(0, result.ScriptCount);
    }

    [Fact]
    public void Create_WithData_ShouldExposeFlagsAndCounts()
    {
        var scripts = new[]
        {
            new RollbackScript("file.sql", "TABLE", "dbo.Test", 1, "DROP TABLE dbo.Test;", [])
        };
        var warnings = new[] { "warning" };
        var assumptions = new[] { "assumption" };

        var result = RollbackGeneration.Create(scripts, warnings, assumptions);

        Assert.Single(result.Scripts);
        Assert.Single(result.Warnings);
        Assert.Single(result.Assumptions);
        Assert.True(result.HasWarnings);
        Assert.True(result.HasAssumptions);
        Assert.Equal(1, result.ScriptCount);
    }
}