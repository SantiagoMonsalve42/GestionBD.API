using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Parametros.Commands;
using GestionBD.Application.Parametros.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ParametrosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParametrosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo parámetro
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateParametroRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateParametroCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todos los parámetros
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ParametroResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var parametros = await _mediator.Send(new GetAllParametrosQuery());
        return Ok(parametros);
    }

    /// <summary>
    /// Obtiene un parámetro por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(ParametroResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var parametro = await _mediator.Send(new GetParametroByIdQuery(id));
        
        if (parametro == null)
            return NotFound(new { message = $"Parámetro con ID {id} no encontrado." });

        return Ok(parametro);
    }

    /// <summary>
    /// Actualiza un parámetro existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateParametroRequest request)
    {
        if (id != request.IdParametro)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateParametroCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un parámetro
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteParametroCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}