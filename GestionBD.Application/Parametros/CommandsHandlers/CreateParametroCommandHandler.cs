using GestionBD.Application.Parametros.Commands;
using GestionBD.Domain;
using GestionBD.Domain.Entities;
using MediatR;

namespace GestionBD.Application.Parametros.CommandsHandlers;

public sealed class CreateParametroCommandHandler : IRequestHandler<CreateParametroCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateParametroCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateParametroCommand command, CancellationToken cancellationToken)
    {
        var parametro = new TblParametro
        {
            NombreParametro = command.Request.NombreParametro,
            ValorNumerico = command.Request.ValorNumerico,
            ValorString = command.Request.ValorString
        };

        _unitOfWork.Parametros.Add(parametro);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return parametro.IdParametro;
    }
}