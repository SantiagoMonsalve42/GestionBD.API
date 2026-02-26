using GestionBD.API.Controllers;
using GestionBD.Application.DTO;
using GestionBD.Application.Statistics.Queries;
using GestionBD.UnitTests.API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.UnitTests.API.Controllers;

public sealed class EstadisticasControllerTests
{
    [Fact]
    public async Task GetDashboardStatistics_ShouldReturnOkWithDto()
    {
        var mediator = new TestMediator();
        mediator.Register<GetDashboardStatisticsQuery, DashboardStatisticsDto>(_ => new DashboardStatisticsDto(1, 2, 3));

        var controller = new EstadisticasController(mediator);

        var result = await controller.GetDashboardStatistics();

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<DashboardStatisticsDto>(ok.Value);

        Assert.Equal(1, dto.CantidadArtefactos);
        Assert.Equal(2, dto.CantidadEntregables);
        Assert.Equal(3, dto.CantidadInstancias);
    }
}