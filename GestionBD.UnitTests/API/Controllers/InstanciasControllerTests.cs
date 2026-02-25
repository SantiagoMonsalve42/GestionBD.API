using GestionBD.API.Controllers;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Instancias.Commands;
using GestionBD.Application.Instancias.Queries;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class InstanciasControllerTests
{
    [Fact]
    public async Task Create_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new InstanciasController(new TestMediator());
        controller.ModelState.AddModelError("Instancia", "Requerido");

        var request = new CreateInstanciaRequest(1, "instancia", 1433, "user", "pass", "db");

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GetAllInstanciasQuery, IEnumerable<InstanciaResponse>>(_ =>
        [
            new InstanciaResponse(1, 1, "instancia", 1433, "user", "db")
        ]);

        var controller = new InstanciasController(mediator);

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetInstanciaByIdQuery, InstanciaResponse?>(_ => null);

        var controller = new InstanciasController(mediator);

        var result = await controller.GetById(10);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var controller = new InstanciasController(new TestMediator());

        var request = new UpdateInstanciaRequest(2, 1, "instancia", 1433, "user", "pass", "db");

        var result = await controller.Update(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<DeleteInstanciaCommand, Unit>(_ => throw new KeyNotFoundException("not found"));

        var controller = new InstanciasController(mediator);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}