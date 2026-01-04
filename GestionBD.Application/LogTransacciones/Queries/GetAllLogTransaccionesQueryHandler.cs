using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.LogTransacciones;

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