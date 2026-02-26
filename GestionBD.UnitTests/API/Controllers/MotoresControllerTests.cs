using GestionBD.API.Controllers;
using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Motores.Commands;
using GestionBD.Application.Motores.Queries;
using GestionBD.UnitTests.API.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class MotoresControllerTests
{
    [Fact]
    public async Task Create_WhenModelStateInvalid_ShouldReturnBadRequest()
    {
        var controller = new MotoresController(new TestMediator());
        controller.ModelState.AddModelError("NombreMotor", "Requerido");

        var request = new CreateMotorRequest("Motor", "1.0", "desc");

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        var mediator = new TestMediator();
        mediator.Register<GetAllMotoresQuery, IEnumerable<MotorResponse>>(_ =>
        [
            new MotorResponse(1, "Motor", "1.0", "desc")
        ]);

        var controller = new MotoresController(mediator);

        var result = await controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<GetMotorByIdQuery, MotorResponse?>(_ => null);

        var controller = new MotoresController(mediator);

        var result = await controller.GetById(10);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_WhenIdMismatch_ShouldReturnBadRequest()
    {
        var controller = new MotoresController(new TestMediator());

        var request = new UpdateMotorRequest(2, "Motor", "1.0", "desc");

        var result = await controller.Update(1, request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        var mediator = new TestMediator();
        mediator.Register<DeleteMotorCommand, Unit>(_ => throw new KeyNotFoundException("not found"));

        var controller = new MotoresController(mediator);

        var result = await controller.Delete(1);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}