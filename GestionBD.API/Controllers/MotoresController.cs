using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Motores.Commands;
using GestionBD.Application.Motores.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionBD.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MotoresController : ControllerBase
{
    private readonly IMediator _mediator;

    public MotoresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo motor de base de datos
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMotorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _mediator.Send(new CreateMotorCommand(request));
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Obtiene todos los motores
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MotorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var motores = await _mediator.Send(new GetAllMotoresQuery());
        return Ok(motores);
    }

    /// <summary>
    /// Obtiene un motor por ID
    /// </summary>
    [HttpGet("{id:decimal}")]
    [ProducesResponseType(typeof(MotorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(decimal id)
    {
        var motor = await _mediator.Send(new GetMotorByIdQuery(id));
        
        if (motor == null)
            return NotFound(new { message = $"Motor con ID {id} no encontrado." });

        return Ok(motor);
    }

    /// <summary>
    /// Actualiza un motor existente
    /// </summary>
    [HttpPut("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(decimal id, [FromBody] UpdateMotorRequest request)
    {
        if (id != request.IdMotor)
            return BadRequest(new { message = "El ID de la URL no coincide con el ID del request." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _mediator.Send(new UpdateMotorCommand(request));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un motor
    /// </summary>
    [HttpDelete("{id:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(decimal id)
    {
        try
        {
            await _mediator.Send(new DeleteMotorCommand(id));
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}