using System.IO.Compression;
using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Services;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using Moq;

namespace GestionBD.UnitTests.Application.Services;

public sealed class EntregableDeploymentServiceTests
{
    [Fact]
    public async Task PreDeployAsync_ValidRequest_ReturnsResultsAndCommits()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregableId = 1m;

            var entregable = new EntregableResponseEstado(
                entregableId,
                zipPath,
                "Desc",
                1m,
                1,
                null,
                "tempdb",
                "Estado",
                1,
                null,
                null);

            var artefactos = new[]
            {
                new ArtefactoResponse(1m, entregableId, 1, "UTF-8", "script.sql", "script.sql", false, null)
            };

            var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
            var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
            entregableReadRepositoryMock
                .Setup(x => x.GetByIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entregable);

            var artefactoReadRepositoryMock = new Mock<IArtefactoReadRepository>();
            artefactoReadRepositoryMock
                .Setup(x => x.GetByEntregableIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(artefactos);

            var scriptExecutorMock = new Mock<IScriptExecutor>();

            var entregableRepositoryMock = new Mock<IEntregableRepository>();
            entregableRepositoryMock
                .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.PreDespliegue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

            var service = new EntregableDeploymentService(
                entregableReadRepositoryMock.Object,
                artefactoReadRepositoryMock.Object,
                scriptExecutorMock.Object,
                Mock.Of<IDacpacService>(),
                Mock.Of<IInstanciaReadRepository>(),
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            var results = (await service.PreDeployAsync(entregableId, CancellationToken.None)).ToList();

            Assert.Single(results);
            Assert.True(results[0].IsValid);
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task PreDeployAsync_MissingTemporalDb_ThrowsAndRollsBack()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregable = new EntregableResponseEstado(
                1m,
                zipPath,
                "Desc",
                1m,
                1,
                null,
                null,
                "Estado",
                1,
                null,
                null);

            var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
            var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
            entregableReadRepositoryMock
                .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entregable);

            var unitOfWorkMock = new Mock<IUnitOfWork>();

            var service = new EntregableDeploymentService(
                entregableReadRepositoryMock.Object,
                Mock.Of<IArtefactoReadRepository>(),
                Mock.Of<IScriptExecutor>(),
                Mock.Of<IDacpacService>(),
                Mock.Of<IInstanciaReadRepository>(),
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.PreDeployAsync(1m, CancellationToken.None));

            unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task PreDeployAsync_InvalidScriptAndRutaDacpac_CallsDeployDacpac()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregableId = 1m;

            var entregable = new EntregableResponseEstado(
                entregableId,
                zipPath,
                "Desc",
                1m,
                1,
                "dacpac",
                "tempdb",
                "Estado",
                1,
                null,
                null);

            var artefactos = new[]
            {
                new ArtefactoResponse(1m, entregableId, 1, "UTF-8", "script.sql", "script.sql", false, null)
            };

            var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
            entregableReadRepositoryMock
                .Setup(x => x.GetByIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entregable);

            var artefactoReadRepositoryMock = new Mock<IArtefactoReadRepository>();
            artefactoReadRepositoryMock
                .Setup(x => x.GetByEntregableIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(artefactos);

            var scriptExecutorMock = new Mock<IScriptExecutor>();
            scriptExecutorMock
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), null, null, null, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var dacpacServiceMock = new Mock<IDacpacService>();

            var entregableRepositoryMock = new Mock<IEntregableRepository>();
            entregableRepositoryMock
                .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.PreDespliegue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
            unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

            var service = new EntregableDeploymentService(
                entregableReadRepositoryMock.Object,
                artefactoReadRepositoryMock.Object,
                scriptExecutorMock.Object,
                dacpacServiceMock.Object,
                Mock.Of<IInstanciaReadRepository>(),
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            var results = (await service.PreDeployAsync(entregableId, CancellationToken.None)).ToList();

            Assert.Single(results);
            Assert.False(results[0].IsValid);
            dacpacServiceMock.Verify(x => x.DeployDacpacToTemporaryDatabaseAsync("dacpac", "tempdb", It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task DeployAsync_ValidRequest_ReturnsResultsAndCommits()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregableId = 2m;

            var entregable = new EntregableResponseEstado(
                entregableId,
                zipPath,
                "Desc",
                1m,
                1,
                null,
                null,
                "Estado",
                1,
                null,
                null);

            var artefactos = new[]
            {
                new ArtefactoResponse(1m, entregableId, 1, "UTF-8", "script.sql", "script.sql", false, null)
            };

            var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
            entregableReadRepositoryMock
                .Setup(x => x.GetByIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entregable);

            var artefactoReadRepositoryMock = new Mock<IArtefactoReadRepository>();
            artefactoReadRepositoryMock
                .Setup(x => x.GetByEntregableIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(artefactos);

            var instanciaReadRepositoryMock = new Mock<IInstanciaReadRepository>();
            instanciaReadRepositoryMock
                .Setup(x => x.GetConnectionDetailsByEntregableIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InstanciaConnectResponse("srv", "usr", 1433, "db", null));

            var scriptExecutorMock = new Mock<IScriptExecutor>();

            var entregableRepositoryMock = new Mock<IEntregableRepository>();
            entregableRepositoryMock
                .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.Despliegue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
            keyVaultProvider
                .Setup(x => x.GetSecretsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, object>
                {
                    ["user"] = "usr",
                    ["pass"] = "pwd"
                });
            unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

            var service = new EntregableDeploymentService(
                entregableReadRepositoryMock.Object,
                artefactoReadRepositoryMock.Object,
                scriptExecutorMock.Object,
                Mock.Of<IDacpacService>(),
                instanciaReadRepositoryMock.Object,
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            var results = (await service.DeployAsync(entregableId, CancellationToken.None)).ToList();

            Assert.Single(results);
            Assert.True(results[0].IsValid);
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task DeployAsync_MissingConnectionDetails_ThrowsAndRollsBack()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregable = new EntregableResponseEstado(
                1m,
                zipPath,
                "Desc",
                1m,
                1,
                null,
                null,
                "Estado",
                1,
                null,
                null);

            var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
            entregableReadRepositoryMock
                .Setup(x => x.GetByIdAsync(1m, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entregable);

            var instanciaReadRepositoryMock = new Mock<IInstanciaReadRepository>();
            instanciaReadRepositoryMock
                .Setup(x => x.GetConnectionDetailsByEntregableIdAsync(1m, It.IsAny<CancellationToken>()))
                .ReturnsAsync((InstanciaConnectResponse?)null);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
            var service = new EntregableDeploymentService(
                entregableReadRepositoryMock.Object,
                Mock.Of<IArtefactoReadRepository>(),
                Mock.Of<IScriptExecutor>(),
                Mock.Of<IDacpacService>(),
                instanciaReadRepositoryMock.Object,
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.DeployAsync(1m, CancellationToken.None));

            unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    private static string CreateZipWithScript(string fileName, string content)
    {
        var zipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.zip");

        using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
        var entry = archive.CreateEntry(fileName);
        using var writer = new StreamWriter(entry.Open());
        writer.Write(content);

        return zipPath;
    }
}