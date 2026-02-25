using GestionBD.API.Controllers;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Artefactos.Queries;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class ArtefactosControllerTests
{
    [Fact]
    public async Task Create_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new ArtefactosController(new TestMediator());
        controller.ModelState.AddModelError("Codificacion", "Requerido");

        var request = new CreateArtefactoRequest(
            1,
            1,
            "UTF-8",
            "script.sql",
            "scripts/script.sql",
            false);

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GetAllArtefactosQuery, IEnumerable<ArtefactoResponse>>(_ =>
        [
            new ArtefactoResponse(1, 1, 1, "UTF-8", "script.sql", "scripts/script.sql", false, "desc")
        ]);

        var controller = new ArtefactosController(mediator);

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetArtefactoByIdQuery, ArtefactoResponse?>(_ => null);

        var controller = new ArtefactosController(mediator);

        var result = await controller.GetById(10);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var controller = new ArtefactosController(new TestMediator());

        var request = new UpdateArtefactoRequest(2, 1, 1, "UTF-8", "script.sql", "scripts/script.sql", false);

        var result = await controller.Update(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<DeleteArtefactoCommand, Unit>(_ => throw new KeyNotFoundException("not found"));

        var controller = new ArtefactosController(mediator);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetByEntregableId_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetArtefactoByEntregableIdQuery, IEnumerable<ArtefactoResponse>?>(_ => null);

        var controller = new ArtefactosController(mediator);

        var result = await controller.GetByEntregableId(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task UpdateRangeOrder_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new ArtefactosController(new TestMediator());
        controller.ModelState.AddModelError("request", "invalid");

        var result = await controller.UpdateRangeOrder([new ArtefactoChangeOrder(1, 2)]);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}