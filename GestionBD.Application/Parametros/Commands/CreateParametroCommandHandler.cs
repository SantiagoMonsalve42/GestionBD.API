using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.Parametros.Commands;

public sealed class CreateParametroCommandHandler : IRequestHandler<CreateParametroCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateParametroCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CreateParametroCommand command, CancellationToken cancellationToken)
    {
        var parametro = new TblParametro
        {
            IdParametro = command.Request.IdParametro,
            NombreParametro = command.Request.NombreParametro,
            ValorNumerico = command.Request.ValorNumerico,
            ValorString = command.Request.ValorString
        };

        _context.TblParametros.Add(parametro);
        await _context.SaveChangesAsync(cancellationToken);

        return parametro.IdParametro;
    }
}