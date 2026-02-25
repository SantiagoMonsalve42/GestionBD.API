using GestionBD.API.Controllers;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.LogEventos.Commands;
using GestionBD.Application.LogEventos.Queries;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class LogEventosControllerTests
{
    [Fact]
    public async Task Create_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new LogEventosController(new TestMediator());
        controller.ModelState.AddModelError("Descripcion", "Requerido");

        var request = new CreateLogEventoRequest(1, DateTime.UtcNow, "desc", "OK");

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GetAllLogEventosQuery, IEnumerable<LogEventoResponse>>(_ =>
        [
            new LogEventoResponse(1, 1, DateTime.UtcNow, "desc", "OK", "transaccion")
        ]);

        var controller = new LogEventosController(mediator);

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetLogEventoByIdQuery, LogEventoResponse?>(_ => null);

        var controller = new LogEventosController(mediator);

        var result = await controller.GetById(10);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var controller = new LogEventosController(new TestMediator());

        var request = new UpdateLogEventoRequest(2, 1, DateTime.UtcNow, "desc", "OK");

        var result = await controller.Update(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<DeleteLogEventoCommand, Unit>(_ => throw new KeyNotFoundException("not found"));

        var controller = new LogEventosController(mediator);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}