using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Instancias.Commands;
using GestionBD.Application.Instancias.Helpers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Instancias.CommandsHandlers;

public sealed class UpdateInstanciaCommandHandler : IRequestHandler<UpdateInstanciaCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVaultConfigurationProvider _vaultConfigurationProvider;

    public UpdateInstanciaCommandHandler(IUnitOfWork unitOfWork, IVaultConfigurationProvider vaultConfigurationProvider)
    {
        _unitOfWork = unitOfWork;
        _vaultConfigurationProvider = vaultConfigurationProvider;
    }

    public async Task<Unit> Handle(UpdateInstanciaCommand command, CancellationToken cancellationToken)
    {
        var instancia = await _unitOfWork.FindEntityAsync<TblInstancia>(command.Request.IdInstancia, cancellationToken);

        if (instancia == null)
            throw new KeyNotFoundException($"Instancia con ID {command.Request.IdInstancia} no encontrada.");

        var vaultPath = await VaultPathHelper.CreateVaultPathAsync(
            _vaultConfigurationProvider,
            command.Request.Usuario,
            command.Request.Password,
            instancia.IdInstancia,
            cancellationToken);

        instancia.IdMotor = command.Request.IdMotor;
        instancia.Instancia = command.Request.Instancia;
        instancia.Puerto = command.Request.Puerto;
        instancia.SessionPath = vaultPath;
        instancia.NombreDB = command.Request.nombreBD;

        _unitOfWork.Instancias.Update(instancia);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}