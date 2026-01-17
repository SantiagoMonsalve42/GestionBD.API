using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using GestionBD.Application.Instancias.Commands;

namespace GestionBD.Application.Instancias.CommandsHandlers;

public sealed class UpdateInstanciaCommandHandler : IRequestHandler<UpdateInstanciaCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateInstanciaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateInstanciaCommand command, CancellationToken cancellationToken)
    {
        var instancia = await _unitOfWork.FindEntityAsync<TblInstancia>(command.Request.IdInstancia, cancellationToken);

        if (instancia == null)
            throw new KeyNotFoundException($"Instancia con ID {command.Request.IdInstancia} no encontrada.");

        instancia.IdMotor = command.Request.IdMotor;
        instancia.Instancia = command.Request.Instancia;
        instancia.Puerto = command.Request.Puerto;
        instancia.Usuario = command.Request.Usuario;
        instancia.Password = command.Request.Password;
        instancia.NombreDB = command.Request.nombreBD;

        _unitOfWork.Instancias.Update(instancia);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}