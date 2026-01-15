using MediatR;
using GestionBD.Application.Contracts.LogTransacciones;
using GestionBD.Application.Abstractions.Repositories.Query;

namespace GestionBD.Application.LogTransacciones.Queries;

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