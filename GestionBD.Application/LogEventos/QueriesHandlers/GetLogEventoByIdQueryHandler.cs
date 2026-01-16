using MediatR;
using GestionBD.Application.Contracts.LogEventos;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.LogEventos.Queries;

namespace GestionBD.Application.LogEventos.QueriesHandlers;

public sealed class GetLogEventoByIdQueryHandler : IRequestHandler<GetLogEventoByIdQuery, LogEventoResponse?>
{
    private readonly ILogEventoReadRepository _repository;

    public GetLogEventoByIdQueryHandler(ILogEventoReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<LogEventoResponse?> Handle(GetLogEventoByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdEvento, cancellationToken);
    }
}