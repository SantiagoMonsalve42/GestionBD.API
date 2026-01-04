using MediatR;
using GestionBD.Domain;
using GestionBD.Domain.Entities;

namespace GestionBD.Application.LogEventos.Commands;

public sealed class CreateLogEventoCommandHandler : IRequestHandler<CreateLogEventoCommand, decimal>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateLogEventoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(CreateLogEventoCommand command, CancellationToken cancellationToken)
    {
        var logEvento = new TblLogEvento
        {
            IdTransaccion = command.Request.IdTransaccion,
            FechaEjecucion = command.Request.FechaEjecucion,
            Descripcion = command.Request.Descripcion,
            EstadoEvento = command.Request.EstadoEvento
        };

        _unitOfWork.LogEventos.Add(logEvento);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return logEvento.IdEvento;
    }
}