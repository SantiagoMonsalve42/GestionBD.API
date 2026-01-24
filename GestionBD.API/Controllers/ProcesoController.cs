using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Entregables.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class ProcesoController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProcesoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("first-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateEntregableFile(decimal idEntregable)
    {
        var isValid = await _mediator.Send(new EntregableEfimeroCommand(idEntregable));
        return Ok(new { isValid });
    }
    [HttpPost("second-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateArtefactos(decimal idEntregable)
    {
        var isValid = await _mediator.Send(new ValidateArtefactoCommand(idEntregable));
        return Ok(isValid);
    }

    [HttpPost("third-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PreDeployEntregableFile(decimal idEntregable)
    {
        var isValid = await _mediator.Send(new DesplegarEntregableEfimeroCommand(idEntregable));
        return Ok(isValid);
    }

    [HttpPost("fourth-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerarRollback(decimal idEntregable)
    {
        //var isValid = await _mediator.Send(new DesplegarEntregableCommand(idEntregable));
        return Ok("TODO");
    }

    [HttpPost("fifth-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeployEntregableFile(decimal idEntregable)
    {
        var isValid = await _mediator.Send(new DesplegarEntregableCommand(idEntregable));
        return Ok(new { isValid });
    }

    [HttpPost("sixth-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SentToRevision(decimal idEntregable)
    {
        var isValid = await _mediator.Send(new EntregableToRevisionCommand(idEntregable));
        return NoContent();
    }

    [HttpPost("seventh-step/{idEntregable:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SentToCerrado(decimal idEntregable)
    {
        var isValid = await _mediator.Send(new EntregableToCerradoCommand(idEntregable));
        return NoContent();
    }
}
