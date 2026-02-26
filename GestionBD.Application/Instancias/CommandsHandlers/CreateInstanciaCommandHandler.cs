using GestionBD.Application.Abstractions.Config;
using GestionBD.Application.Instancias.Commands;
using GestionBD.Application.Instancias.Helpers;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Instancias.CommandsHandlers;

public sealed class CreateInstanciaCommandHandler : IRequestHandler<CreateInstanciaCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVaultConfigurationProvider _vaultConfigurationProvider;

    public CreateInstanciaCommandHandler(IUnitOfWork unitOfWork, IVaultConfigurationProvider vaultConfigurationProvider)
    {
        _unitOfWork = unitOfWork;
        _vaultConfigurationProvider = vaultConfigurationProvider;
    }

    public async Task<decimal> Handle(CreateInstanciaCommand command, CancellationToken cancellationToken)
    {
        

        var instancia = new TblInstancia
        {
            IdMotor = command.Request.IdMotor,
            Instancia = command.Request.Instancia,
            Puerto = command.Request.Puerto,
            SessionPath = "",
            NombreDB = command.Request.nombreBD
        };

        _unitOfWork.Instancias.Add(instancia);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var vaultPath = await VaultPathHelper.CreateVaultPathAsync(
            _vaultConfigurationProvider,
            command.Request.Usuario,
            command.Request.Password,
            instancia.IdInstancia,
            cancellationToken);

        instancia.SessionPath = vaultPath;

        _unitOfWork.Instancias.Update(instancia);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return instancia.IdInstancia;
    }
}