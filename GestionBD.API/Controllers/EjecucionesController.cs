using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionBD.Application.Ejecuciones.Commands;
using GestionBD.Application.Ejecuciones.Queries;
using GestionBD.Application.Contracts.Ejecuciones;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EjecucionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EjecucionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea una nueva ejecución
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEjecucionRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateEjecucionCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todas las ejecuciones
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EjecucionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var ejecuciones = await _mediator.Send(new GetAllEjecucionesQuery());
        return Ok(ejecuciones);
    }

    /// <summary>
    /// Obtiene una ejecución por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(EjecucionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var ejecucion = await _mediator.Send(new GetEjecucionByIdQuery(id));
        
        if (ejecucion == null)
            return NotFound(new { message = $"Ejecución con ID {id} no encontrada." });

        return Ok(ejecucion);
    }

    /// <summary>
    /// Actualiza una ejecución existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateEjecucionRequest request)
    {
        if (id != request.IdEjecucion)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateEjecucionCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una ejecución
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteEjecucionCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}