using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.LogEventos.Commands;
using GestionBD.Application.LogEventos.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class LogEventosController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogEventosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo log de evento
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateLogEventoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateLogEventoCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todos los logs de eventos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LogEventoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var logEventos = await _mediator.Send(new GetAllLogEventosQuery());
        return Ok(logEventos);
    }

    /// <summary>
    /// Obtiene un log de evento por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(LogEventoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var logEvento = await _mediator.Send(new GetLogEventoByIdQuery(id));

        if (logEvento == null)
            return NotFound(new { message = $"Log de evento con ID {id} no encontrado." });

        return Ok(logEvento);
    }

    /// <summary>
    /// Actualiza un log de evento existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateLogEventoRequest request)
    {
        if (id != request.IdEvento)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateLogEventoCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un log de evento
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteLogEventoCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}