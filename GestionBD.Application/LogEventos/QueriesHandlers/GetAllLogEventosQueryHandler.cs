using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.LogEventos.Queries;
using MediatR;

namespace GestionBD.Application.LogEventos.QueriesHandlers;

public sealed class GetAllLogEventosQueryHandler : IRequestHandler<GetAllLogEventosQuery, IEnumerable<LogEventoResponse>>
{
    private readonly ILogEventoReadRepository _repository;

    public GetAllLogEventosQueryHandler(ILogEventoReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LogEventoResponse>> Handle(GetAllLogEventosQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}