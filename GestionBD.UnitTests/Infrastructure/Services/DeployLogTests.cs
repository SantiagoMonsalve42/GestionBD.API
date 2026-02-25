using GestionBD.Application.Contracts.Entregables;
using GestionBD.Infrastructure.Services;
using Xunit;

namespace GestionBD.UnitTests.Infrastructure.Services;

public sealed class DeployLogTests
{
    [Fact]
    public async Task GenerarArchivoLog_NullResponses_ThrowsArgumentNullException()
    {
        var service = new DeployLog();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.GenerarArchivoLog(null!, "C:\\temp\\file.zip", "log.txt"));
    }

    [Fact]
    public async Task GenerarArchivoLog_EmptyPath_ThrowsArgumentException()
    {
        var service = new DeployLog();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GenerarArchivoLog([], "", "log.txt"));
    }

    [Fact]
    public async Task GenerarArchivoLog_ValidInput_CreatesFileAndReturnsPath()
    {
        var service = new DeployLog();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            var responses = new[]
            {
                new EntregablePreValidateResponse(true, "script.sql", "Success", null, null)
            };

            var ruta = Path.Combine(tempDir, "entregable.zip");
            var result = await service.GenerarArchivoLog(responses, ruta, "deploy-log.txt");

            Assert.True(File.Exists(result));
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}