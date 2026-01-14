using MediatR;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Abstractions.Readers;

namespace GestionBD.Application.Entregables.Queries;

public sealed class GetEntregableByIdQueryHandler : IRequestHandler<GetEntregableByIdQuery, EntregableResponse?>
{
    private readonly IEntregableReadRepository _repository;

    public GetEntregableByIdQueryHandler(IEntregableReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<EntregableResponse?> Handle(GetEntregableByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdEntregable, cancellationToken);
    }
}