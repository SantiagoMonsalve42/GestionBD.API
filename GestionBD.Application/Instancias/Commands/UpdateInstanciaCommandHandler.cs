using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Instancias.Commands;

public sealed class UpdateInstanciaCommandHandler : IRequestHandler<UpdateInstanciaCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateInstanciaCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateInstanciaCommand command, CancellationToken cancellationToken)
    {
        var instancia = await _context.TblInstancias
            .FirstOrDefaultAsync(i => i.IdInstancia == command.Request.IdInstancia, cancellationToken);

        if (instancia == null)
            throw new KeyNotFoundException($"Instancia con ID {command.Request.IdInstancia} no encontrada.");

        instancia.IdMotor = command.Request.IdMotor;
        instancia.Instancia = command.Request.Instancia;
        instancia.Puerto = command.Request.Puerto;
        instancia.Usuario = command.Request.Usuario;
        instancia.Password = command.Request.Password;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}