using GestionBD.API.Controllers;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Commands;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class ProcesoControllerTests
{
    [Fact]
    public async Task ValidateEntregableFile_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<EntregableEfimeroCommand, string>(_ => "ok");

        var controller = new ProcesoController(mediator);

        var result = await controller.ValidateEntregableFile(5);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ValidateArtefactos_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<ValidateArtefactoCommand, IEnumerable<ValidateArtefactoResponse>>(_ =>
        [
            new ValidateArtefactoResponse("script.sql", GestionBD.Domain.ValueObjects.SqlValidation.Valid())
        ]);

        var controller = new ProcesoController(mediator);

        var result = await controller.ValidateArtefactos(5);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task PreDeployEntregableFile_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<DesplegarEntregableEfimeroCommand, IEnumerable<EntregablePreValidateResponse>>(_ =>
        [
            new EntregablePreValidateResponse(true, "script", "OK", null, null)
        ]);

        var controller = new ProcesoController(mediator);

        var result = await controller.PreDeployEntregableFile(5);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GenerarRollback_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GenerateRollbackCommand, string?>(_ => "ok");

        var controller = new ProcesoController(mediator);

        var result = await controller.GenerarRollback(5);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task DeployEntregableFile_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<DesplegarEntregableCommand, string>(_ => "ok");

        var controller = new ProcesoController(mediator);

        var result = await controller.DeployEntregableFile(5);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task SentToRevision_ShouldReturnNoContent()
    {
        var mediator = new TestMediator();
        mediator.Register<EntregableToRevisionCommand, Unit>(_ => Unit.Value);

        var controller = new ProcesoController(mediator);

        var result = await controller.SentToRevision(5);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task SentToCerrado_ShouldReturnNoContent()
    {
        var mediator = new TestMediator();
        mediator.Register<EntregableToCerradoCommand, Unit>(_ => Unit.Value);

        var controller = new ProcesoController(mediator);

        var result = await controller.SentToCerrado(5, 1);

        Assert.IsType<NoContentResult>(result);
    }
}