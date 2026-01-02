using MediatR;
using GestionBD.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionBD.Application.Parametros.Commands;

public sealed class UpdateParametroCommandHandler : IRequestHandler<UpdateParametroCommand, Unit>
{
    private readonly ApplicationDbContext _context;

    public UpdateParametroCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateParametroCommand command, CancellationToken cancellationToken)
    {
        var parametro = await _context.TblParametros
            .FirstOrDefaultAsync(p => p.IdParametro == command.Request.IdParametro, cancellationToken);

        if (parametro == null)
            throw new KeyNotFoundException($"Parámetro con ID {command.Request.IdParametro} no encontrado.");

        parametro.NombreParametro = command.Request.NombreParametro;
        parametro.ValorNumerico = command.Request.ValorNumerico;
        parametro.ValorString = command.Request.ValorString;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}