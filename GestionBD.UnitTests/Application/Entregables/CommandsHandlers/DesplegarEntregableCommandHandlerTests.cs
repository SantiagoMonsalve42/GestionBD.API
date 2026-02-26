using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Abstractions.Services;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.CommandsHandlers;
using GestionBD.Application.Services;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using GestionBD.Domain.Exceptions;
using Moq;
using System.Collections.Generic;
using System.IO.Compression;

namespace GestionBD.UnitTests.Application.Entregables.CommandsHandlers;

public sealed class DesplegarEntregableCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsLogPathMessage()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregableId = 3m;
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

            var entregableRepositoryMock = new Mock<IEntregableRepository>();
            entregableRepositoryMock
                .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.Despliegue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            entregableRepositoryMock
                .Setup(x => x.UpdateRutaResultado(entregableId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
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

            var deploymentService = new EntregableDeploymentService(
                entregableReadRepositoryMock.Object,
                artefactoReadRepositoryMock.Object,
                Mock.Of<IScriptExecutor>(),
                Mock.Of<IDacpacService>(),
                instanciaReadRepositoryMock.Object,
                unitOfWorkMock.Object,
                keyVaultProvider.Object);

            var deployLogMock = new Mock<IDeployLog>();
            deployLogMock
                .Setup(x => x.GenerarArchivoLog(It.IsAny<IEnumerable<EntregablePreValidateResponse>>(), zipPath, It.IsAny<string>()))
                .ReturnsAsync("resultPath");

            var handler = new DesplegarEntregableCommandHandler(
                deploymentService,
                deployLogMock.Object,
                entregableReadRepositoryMock.Object,
                unitOfWorkMock.Object);

            var result = await handler.Handle(new DesplegarEntregableCommand(entregableId), CancellationToken.None);

            Assert.Contains("resultPath", result);
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task Handle_EntregableNotFound_ThrowsValidationException()
    {
        var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
        entregableReadRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntregableResponseEstado?)null);
        var keyVaultProvider = new Mock<IVaultConfigurationProvider>();
        var deploymentService = new EntregableDeploymentService(
            entregableReadRepositoryMock.Object,
            Mock.Of<IArtefactoReadRepository>(),
            Mock.Of<IScriptExecutor>(),
            Mock.Of<IDacpacService>(),
            Mock.Of<IInstanciaReadRepository>(),
            Mock.Of<IUnitOfWork>(),
            keyVaultProvider.Object);

        var handler = new DesplegarEntregableCommandHandler(
            deploymentService,
            Mock.Of<IDeployLog>(),
            entregableReadRepositoryMock.Object,
            Mock.Of<IUnitOfWork>());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new DesplegarEntregableCommand(1m), CancellationToken.None));
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