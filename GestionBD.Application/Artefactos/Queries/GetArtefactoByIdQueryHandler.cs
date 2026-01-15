using MediatR;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Abstractions.Repositories.Query;

namespace GestionBD.Application.Artefactos.Queries;

public sealed class GetArtefactoByIdQueryHandler : IRequestHandler<GetArtefactoByIdQuery, ArtefactoResponse?>
{
    private readonly IArtefactoReadRepository _repository;

    public GetArtefactoByIdQueryHandler(IArtefactoReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<ArtefactoResponse?> Handle(GetArtefactoByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdArtefacto, cancellationToken);
    }
}