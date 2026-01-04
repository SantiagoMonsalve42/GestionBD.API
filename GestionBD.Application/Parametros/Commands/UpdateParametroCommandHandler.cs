using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.Parametros.Commands;

public sealed class UpdateParametroCommandHandler : IRequestHandler<UpdateParametroCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateParametroCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateParametroCommand command, CancellationToken cancellationToken)
    {
        var parametro = await _unitOfWork.FindEntityAsync<TblParametro>(command.Request.IdParametro, cancellationToken);

        if (parametro == null)
            throw new KeyNotFoundException($"Parámetro con ID {command.Request.IdParametro} no encontrado.");

        parametro.NombreParametro = command.Request.NombreParametro;
        parametro.ValorNumerico = command.Request.ValorNumerico;
        parametro.ValorString = command.Request.ValorString;

        _unitOfWork.Parametros.Update(parametro);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}