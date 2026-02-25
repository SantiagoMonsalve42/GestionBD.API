using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.LogTransacciones;
using GestionBD.Application.LogTransacciones.Queries;
using MediatR;

namespace GestionBD.Application.LogTransacciones.QueriesHandlers;

public sealed class GetAllLogTransaccionesQueryHandler : IRequestHandler<GetAllLogTransaccionesQuery, IEnumerable<LogTransaccionResponse>>
{
    private readonly ILogTransaccionReadRepository _repository;

    public GetAllLogTransaccionesQueryHandler(ILogTransaccionReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LogTransaccionResponse>> Handle(GetAllLogTransaccionesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}