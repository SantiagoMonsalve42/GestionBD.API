using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Artefactos;
using MediatR;

namespace GestionBD.Application.Artefactos.Queries;

public sealed class GetArtefactoByEntregableIdQueryHandler : IRequestHandler<GetArtefactoByEntregableIdQuery, IEnumerable<ArtefactoResponse>>
{
    private readonly IArtefactoReadRepository _repository;
    public GetArtefactoByEntregableIdQueryHandler(IArtefactoReadRepository artefactoReadRepository)
    {
        _repository = artefactoReadRepository;
    }

    public async Task<IEnumerable<ArtefactoResponse>> Handle(GetArtefactoByEntregableIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByEntregableIdAsync(request.IdEntregable, cancellationToken);
    }
}
