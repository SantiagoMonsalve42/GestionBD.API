using GestionBD.Infrastructure.Services;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Services;

public sealed class ScriptRegexServiceTests
{
    [Fact]
    public void GetRelatedObjects_EmptyScript_ReturnsEmpty()
    {
        var service = new ScriptRegexService();

        var result = service.getRelatedObjects("");

        Assert.Empty(result);
    }

    [Fact]
    public void GetRelatedObjects_ValidScript_ReturnsObjects()
    {
        var service = new ScriptRegexService();
        var script = "CREATE TABLE dbo.Users (Id int); SELECT * FROM dbo.Users;";

        var result = service.getRelatedObjects(script);

        Assert.Single(result);
        Assert.Equal("dbo.Users", result[0]);
    }

    [Fact]
    public void GetRelatedObjects_SystemObjects_AreFiltered()
    {
        var service = new ScriptRegexService();
        var script = "SELECT * FROM sys.objects;";

        var result = service.getRelatedObjects(script);

        Assert.Empty(result);
    }
}