using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Instancias.Commands;
using GestionBD.Application.Instancias.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class InstanciasController : ControllerBase
{
    private readonly IMediator _mediator;

    public InstanciasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea una nueva instancia de base de datos
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInstanciaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateInstanciaCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todas las instancias
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InstanciaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var instancias = await _mediator.Send(new GetAllInstanciasQuery());
        return Ok(instancias);
    }

    /// <summary>
    /// Obtiene una instancia por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(InstanciaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var instancia = await _mediator.Send(new GetInstanciaByIdQuery(id));
        
        if (instancia == null)
            return NotFound(new { message = $"Instancia con ID {id} no encontrada." });

        return Ok(instancia);
    }

    /// <summary>
    /// Actualiza una instancia existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateInstanciaRequest request)
    {
        if (id != request.IdInstancia)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateInstanciaCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una instancia
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteInstanciaCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}