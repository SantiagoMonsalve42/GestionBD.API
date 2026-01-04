using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Artefactos;

namespace GestionBD.Application.Artefactos.Queries;

public sealed class GetAllArtefactosQueryHandler : IRequestHandler<GetAllArtefactosQuery, IEnumerable<ArtefactoResponse>>
{
    private readonly IArtefactoReadRepository _repository;

    public GetAllArtefactosQueryHandler(IArtefactoReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ArtefactoResponse>> Handle(GetAllArtefactosQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}