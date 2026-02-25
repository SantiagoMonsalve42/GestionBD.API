using GestionBD.API.Controllers;
using GestionBD.Application.Contracts.LogTransacciones;
using GestionBD.Application.LogTransacciones.Commands;
using GestionBD.Application.LogTransacciones.Queries;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class LogTransaccionesControllerTests
{
    [Fact]
    public async Task Create_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new LogTransaccionesController(new TestMediator());
        controller.ModelState.AddModelError("NombreTransaccion", "Requerido");

        var request = new CreateLogTransaccionRequest(
            "Transaccion",
            "A",
            "desc",
            DateTime.UtcNow,
            "resp",
            null,
            "user");

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GetAllLogTransaccionesQuery, IEnumerable<LogTransaccionResponse>>(_ =>
        [
            new LogTransaccionResponse(1, "Transaccion", "A", "desc", DateTime.UtcNow, null, null, "user")
        ]);

        var controller = new LogTransaccionesController(mediator);

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetLogTransaccionByIdQuery, LogTransaccionResponse?>(_ => null);

        var controller = new LogTransaccionesController(mediator);

        var result = await controller.GetById(10);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var controller = new LogTransaccionesController(new TestMediator());

        var request = new UpdateLogTransaccionRequest(
            2,
            "Transaccion",
            "A",
            "desc",
            DateTime.UtcNow,
            "resp",
            null,
            "user");

        var result = await controller.Update(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<DeleteLogTransaccionCommand, Unit>(_ => throw new KeyNotFoundException("not found"));

        var controller = new LogTransaccionesController(mediator);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}