using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionBD.Application.LogTransacciones.Commands;
using GestionBD.Application.LogTransacciones.Queries;
using GestionBD.Application.Contracts.LogTransacciones;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class LogTransaccionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LogTransaccionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo log de transacción
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateLogTransaccionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateLogTransaccionCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todos los logs de transacciones
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LogTransaccionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var logTransacciones = await _mediator.Send(new GetAllLogTransaccionesQuery());
        return Ok(logTransacciones);
    }

    /// <summary>
    /// Obtiene un log de transacción por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(LogTransaccionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var logTransaccion = await _mediator.Send(new GetLogTransaccionByIdQuery(id));
        
        if (logTransaccion == null)
            return NotFound(new { message = $"Log de transacción con ID {id} no encontrado." });

        return Ok(logTransaccion);
    }

    /// <summary>
    /// Actualiza un log de transacción existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateLogTransaccionRequest request)
    {
        if (id != request.IdTransaccion)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateLogTransaccionCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un log de transacción
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteLogTransaccionCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}