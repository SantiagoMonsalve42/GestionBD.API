using MediatR;
using Microsoft.AspNetCore.Mvc;
using GestionBD.Application.Artefactos.Commands;
using GestionBD.Application.Artefactos.Queries;
using GestionBD.Application.Contracts.Artefactos;
using Microsoft.AspNetCore.Authorization;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ArtefactosController : ControllerBase
{
    private readonly IMediator _mediator;
    public ArtefactosController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /// <summary>
    /// Crea un nuevo artefacto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateArtefactoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateArtefactoCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todos los artefactos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ArtefactoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var artefactos = await _mediator.Send(new GetAllArtefactosQuery());
        return Ok(artefactos);
    }

    /// <summary>
    /// Obtiene un artefacto por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(ArtefactoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var artefacto = await _mediator.Send(new GetArtefactoByIdQuery(id));
        
        if (artefacto == null)
            return NotFound(new { message = $"Artefacto con ID {id} no encontrado." });

        return Ok(artefacto);
    }

    /// <summary>
    /// Actualiza un artefacto existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateArtefactoRequest request)
    {
        if (id != request.IdArtefacto)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateArtefactoCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un artefacto
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteArtefactoCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    [HttpGet("entregable/{id:decimal}")]
    [ProducesResponseType(typeof(IEnumerable<ArtefactoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEntregableId(decimal id)
    {
        var artefacto = await _mediator.Send(new GetArtefactoByEntregableIdQuery(id));

        if (artefacto == null)
            return NotFound(new { message = $"Artefactos con ID {id} no encontrado." });

        return Ok(artefacto);
    }

    /// <summary>
    /// Actualiza un artefacto existente
    /// </summary>
    [HttpPut("cambiarOrden")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRangeOrder([FromBody] List<ArtefactoChangeOrder> request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new ChangeOrderArtefactoCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}