using MediatR;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Entregables.Queries;

namespace GestionBD.Application.Entregables.QueriesHandlers;

public sealed class GetEntregableByIdQueryHandler : IRequestHandler<GetEntregableByIdQuery, EntregableResponseEstado?>
{
    private readonly IEntregableReadRepository _repository;

    public GetEntregableByIdQueryHandler(IEntregableReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<EntregableResponseEstado?> Handle(GetEntregableByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdEntregable, cancellationToken);
    }
}