using MediatR;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.Abstractions.Readers;

namespace GestionBD.Application.LogEventos.Queries;

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