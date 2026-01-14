using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Instancias.Commands;

public sealed class CreateInstanciaCommandHandler : IRequestHandler<CreateInstanciaCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateInstanciaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateInstanciaCommand command, CancellationToken cancellationToken)
    {
        var instancia = new TblInstancia
        {
            IdMotor = command.Request.IdMotor,
            Instancia = command.Request.Instancia,
            Puerto = command.Request.Puerto,
            Usuario = command.Request.Usuario,
            Password = command.Request.Password,
            NombreDB = command.Request.nombreBD
        };

        _unitOfWork.Instancias.Add(instancia);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return instancia.IdInstancia;
    }
}