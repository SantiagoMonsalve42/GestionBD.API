using GestionBD.Application.Entregables.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProcesoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProcesoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("first-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateEntregableFile(decimal idEntregable)
    {
        var isValid = await _mediator.Send(new DesplegarEntregableEfimeroCommand(idEntregable));
        return Ok(new { isValid });
    }
}
