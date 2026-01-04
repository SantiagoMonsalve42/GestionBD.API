using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.LogEventos;

namespace GestionBD.Application.LogEventos.Queries;

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