using MediatR;
using GestionBD.Infraestructure.Data;
using GestionBD.Infraestructure.Data.Entities;

namespace GestionBD.Application.Instancias.Commands;

public sealed class CreateInstanciaCommandHandler : IRequestHandler<CreateInstanciaCommand, decimal>
{
    private readonly ApplicationDbContext _context;

    public CreateInstanciaCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> Handle(CreateInstanciaCommand command, CancellationToken cancellationToken)
    {
        var instancia = new TblInstancia
        {
            IdMotor = command.Request.IdMotor,
            Instancia = command.Request.Instancia,
            Puerto = command.Request.Puerto,
            Usuario = command.Request.Usuario,
            Password = command.Request.Password
        };

        _context.TblInstancias.Add(instancia);
        await _context.SaveChangesAsync(cancellationToken);

        return instancia.IdInstancia;
    }
}