using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Application.Services;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using GestionBD.Domain.Exceptions;
using Moq;
using System.IO.Compression;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class DesplegarEntregableEfimeroCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsPreDeployResults()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregableId = 7m;
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
            entregableReadRepositoryMock
                .Setup(x => x.GetByIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entregable);

            var artefactoReadRepositoryMock = new Mock<IArtefactoReadRepository>();
            artefactoReadRepositoryMock
                .Setup(x => x.GetByEntregableIdAsync(entregableId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(artefactos);

            var entregableRepositoryMock = new Mock<IEntregableRepository>();
            entregableRepositoryMock
                .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.PreDespliegue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
            unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

            var deploymentService = new EntregableDeploymentService(
                entregableReadRepositoryMock.Object,
                artefactoReadRepositoryMock.Object,
                Mock.Of<IScriptExecutor>(),
                Mock.Of<IDacpacService>(),
                Mock.Of<IInstanciaReadRepository>(),
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            var handler = new DesplegarEntregableEfimeroCommandHandler(deploymentService);

            var results = (await handler.Handle(new DesplegarEntregableEfimeroCommand(entregableId), CancellationToken.None)).ToList();

            Assert.Single(results);
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task Handle_MissingTemporaryDatabase_ThrowsValidationException()
    {
        var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
        var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
        entregableReadRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EntregableResponseEstado(
                1m,
                "path.zip",
                "Desc",
                1m,
                1,
                null,
                null,
                "Estado",
                1,
                null,
                null));

        var deploymentService = new EntregableDeploymentService(
            entregableReadRepositoryMock.Object,
            Mock.Of<IArtefactoReadRepository>(),
            Mock.Of<IScriptExecutor>(),
            Mock.Of<IDacpacService>(),
            Mock.Of<IInstanciaReadRepository>(),
            Mock.Of<IUnitOfWork>(),
            keyVaultProvider.Object);

        var handler = new DesplegarEntregableEfimeroCommandHandler(deploymentService);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new DesplegarEntregableEfimeroCommand(1m), CancellationToken.None));
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