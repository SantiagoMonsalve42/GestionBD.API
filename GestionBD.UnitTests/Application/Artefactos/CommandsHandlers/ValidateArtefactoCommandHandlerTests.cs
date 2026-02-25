using System.IO.Compression;
using GestionBD.Application.Abstractions.Repositories.Command;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Artefactos.CommandsHandlers;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Domain;
using GestionBD.Domain.Enum;
using GestionBD.Domain.Services;
using GestionBD.Domain.ValueObjects;
using Moq;

namespace GestionBD.UnitTests.Application.Artefactos.CommandsHandlers;

public sealed class ValidateArtefactoCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsValidations()
    {
        var zipPath = CreateZipWithScript("script.sql", "SELECT 1;");
        try
        {
            var entregableId = 10m;
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

            var sqlValidationServiceMock = new Mock<ISqlValidationService>();
            sqlValidationServiceMock
                .Setup(x => x.ValidateScriptAsync(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(SqlValidation.Valid());

            var entregableRepositoryMock = new Mock<IEntregableRepository>();
            entregableRepositoryMock
                .Setup(x => x.UpdateEstado(entregableId, EstadoEntregaEnum.Analisis, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.SetupGet(x => x.Entregables).Returns(entregableRepositoryMock.Object);

            var handler = new ValidateArtefactoCommandHandler(
                entregableReadRepositoryMock.Object,
                artefactoReadRepositoryMock.Object,
                sqlValidationServiceMock.Object,
                unitOfWorkMock.Object);

            var results = (await handler.Handle(new ValidateArtefactoCommand(entregableId), CancellationToken.None)).ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, r => r.Name == "Secuencial");
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    [Fact]
    public async Task Handle_EntregableNotFound_ThrowsInvalidOperationException()
    {
        var entregableReadRepositoryMock = new Mock<IEntregableReadRepository>();
        entregableReadRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EntregableResponseEstado?)null);

        var handler = new ValidateArtefactoCommandHandler(
            entregableReadRepositoryMock.Object,
            Mock.Of<IArtefactoReadRepository>(),
            Mock.Of<ISqlValidationService>(),
            Mock.Of<IUnitOfWork>());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new ValidateArtefactoCommand(1m), CancellationToken.None));
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