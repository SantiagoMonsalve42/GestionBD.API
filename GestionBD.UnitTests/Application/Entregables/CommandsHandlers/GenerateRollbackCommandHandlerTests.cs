using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using GestionBD.Domain.Services;
using GestionBD.Domain.ValueObjects;
using Moq;
using System.IO.Compression;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class GenerateRollbackCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsGeneratedPathMessage()
    {
        var zipPath = CreateZipWithScript("script.sql", "CREATE TABLE T(Id INT);");
        try
        {
            var entregableId = 5m;
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

            var scriptRegexServiceMock = new Mock<IScriptRegexService>();
            scriptRegexServiceMock.Setup(x => x.getRelatedObjects(It.IsAny<string>())).Returns([]);

            var rollbackGenerationServiceMock = new Mock<IRollbackGenerationService>();
            rollbackGenerationServiceMock
                .Setup(x => x.GenerateRollbackAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(RollbackGeneration.Create([], [], []));

            var rollbackServiceMock = new Mock<IRollbackService>();
            rollbackServiceMock
                .Setup(x => x.GenerateRollbackScriptAsync(It.IsAny<List<RollbackGeneration>>(), zipPath, It.IsAny<CancellationToken>()))
                .ReturnsAsync("rollback.zip");

            var entregableRepositoryMock = new Mock<IEntregableRepository>();
            entregableRepositoryMock
                .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.Rollback, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            entregableRepositoryMock
                .Setup(x => x.UpdateRutaRollback(entregableId, "rollback.zip", It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
            unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

            var handler = new GenerateRollbackCommandHandler(
                scriptRegexServiceMock.Object,
                entregableReadRepositoryMock.Object,
                artefactoReadRepositoryMock.Object,
                Mock.Of<IDatabaseService>(),
                instanciaReadRepositoryMock.Object,
                rollbackGenerationServiceMock.Object,
                rollbackServiceMock.Object,
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            var result = await handler.Handle(new GenerateRollbackCommand(entregableId), CancellationToken.None);

            Assert.Contains("rollback.zip", result);
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task Handle_EntregableNotFound_ReturnsErrorMessage()
    {
        var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
        entregableReadRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntregableResponseEstado?)null);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
        var handler = new GenerateRollbackCommandHandler(
            Mock.Of<IScriptRegexService>(),
            entregableReadRepositoryMock.Object,
            Mock.Of<IArtefactoReadRepository>(),
            Mock.Of<IDatabaseService>(),
            Mock.Of<IInstanciaReadRepository>(),
            Mock.Of<IRollbackGenerationService>(),
            Mock.Of<IRollbackService>(),
            unitOfWorkMock.Object,
            keyVaultProvider.Object);

        var result = await handler.Handle(new GenerateRollbackCommand(1m), CancellationToken.None);

        Assert.Contains("Error al generar el archivo de rollback", result);
        unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
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