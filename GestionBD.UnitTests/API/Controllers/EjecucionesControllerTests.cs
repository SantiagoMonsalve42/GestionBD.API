using GestionBD.API.Controllers;
using GestionBD.Application.Contracts.Ejecuciones;
using GestionBD.Application.Ejecuciones.Commands;
using GestionBD.Application.Ejecuciones.Queries;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class EjecucionesControllerTests
{
    [Fact]
    public async Task Create_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new EjecucionesController(new TestMediator());
        controller.ModelState.AddModelError("NombreRequerimiento", "Requerido");

        var request = new CreateEjecucionRequest(1, "desc", "REQ-1");

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GetAllEjecucionesQuery, IEnumerable<EjecucionResponse>>(_ =>
        [
            new EjecucionResponse(1, 1, DateTime.UtcNow, DateTime.UtcNow, "desc", "REQ-1")
        ]);

        var controller = new EjecucionesController(mediator);

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetEjecucionByIdQuery, EjecucionResponse?>(_ => null);

        var controller = new EjecucionesController(mediator);

        var result = await controller.GetById(10);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var controller = new EjecucionesController(new TestMediator());

        var request = new UpdateEjecucionRequest(2, 1, "desc");

        var result = await controller.Update(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<DeleteEjecucionCommand, Unit>(_ => throw new KeyNotFoundException("not found"));

        var controller = new EjecucionesController(mediator);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}