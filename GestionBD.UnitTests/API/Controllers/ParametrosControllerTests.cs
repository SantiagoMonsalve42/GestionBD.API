using GestionBD.API.Controllers;
using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Parametros.Commands;
using GestionBD.Application.Parametros.Queries;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class ParametrosControllerTests
{
    [Fact]
    public async Task Create_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new ParametrosController(new TestMediator());
        controller.ModelState.AddModelError("NombreParametro", "Requerido");

        var request = new CreateParametroRequest("Parametro", 1, "valor");

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GetAllParametrosQuery, IEnumerable<ParametroResponse>>(_ =>
        [
            new ParametroResponse(1, "Parametro", 1, "valor")
        ]);

        var controller = new ParametrosController(mediator);

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetParametroByIdQuery, ParametroResponse?>(_ => null);

        var controller = new ParametrosController(mediator);

        var result = await controller.GetById(10);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var controller = new ParametrosController(new TestMediator());

        var request = new UpdateParametroRequest(2, "Parametro", 1, "valor");

        var result = await controller.Update(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<DeleteParametroCommand, Unit>(_ => throw new KeyNotFoundException("not found"));

        var controller = new ParametrosController(mediator);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}