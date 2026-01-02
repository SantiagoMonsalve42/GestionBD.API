using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionBD.Application.Entregables.Commands;
using GestionBD.Application.Entregables.Queries;
using GestionBD.Application.Contracts.Entregables;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EntregablesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EntregablesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo entregable
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEntregableRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateEntregableCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todos los entregables
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EntregableResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var entregables = await _mediator.Send(new GetAllEntregablesQuery());
        return Ok(entregables);
    }

    /// <summary>
    /// Obtiene un entregable por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(EntregableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var entregable = await _mediator.Send(new GetEntregableByIdQuery(id));
        
        if (entregable == null)
            return NotFound(new { message = $"Entregable con ID {id} no encontrado." });

        return Ok(entregable);
    }

    /// <summary>
    /// Actualiza un entregable existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateEntregableRequest request)
    {
        if (id != request.IdEntregable)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateEntregableCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un entregable
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteEntregableCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}