using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.LogTransacciones;

namespace GestionBD.Application.LogTransacciones.Queries;

public sealed class GetLogTransaccionByIdQueryHandler : IRequestHandler<GetLogTransaccionByIdQuery, LogTransaccionResponse?>
{
    private readonly ILogTransaccionReadRepository _repository;

    public GetLogTransaccionByIdQueryHandler(ILogTransaccionReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<LogTransaccionResponse?> Handle(GetLogTransaccionByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdTransaccion, cancellationToken);
    }
}