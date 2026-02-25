using GestionBD.Domain.ValueObjects;
using GestionBD.Infrastructure.Services;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Services;

public sealed class RollbackServiceTests
{
    [Fact]
    public async Task GenerateRollbackScriptAsync_EmptyList_ThrowsArgumentException()
    {
        var service = new RollbackService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GenerateRollbackScriptAsync([], "C:\\temp\\file.zip", CancellationToken.None));
    }

    [Fact]
    public async Task GenerateRollbackScriptAsync_ValidScripts_CreatesZip()
    {
        var service = new RollbackService();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            var generation = RollbackGeneration.Create(
                [
                    new RollbackScript(
                        "script.sql",
                        "PROC",
                        "dbo.Test",
                        1,
                        "SELECT 1;",
                        [])
                ],
                [],
                []);

            var inputPath = Path.Combine(tempDir, "input.zip");
            var result = await service.GenerateRollbackScriptAsync([generation], inputPath, CancellationToken.None);

            Assert.True(File.Exists(result));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}