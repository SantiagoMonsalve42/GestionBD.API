using GestionBD.Application.DTO;
using GestionBD.Application.Statistics.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstadisticasController : ControllerBase
{

    private readonly IMediator _mediator;

    public EstadisticasController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatisticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStatistics()
    {
        var result = await _mediator.Send(new GetDashboardStatisticsQuery());
        return Ok(result);
    }
}
