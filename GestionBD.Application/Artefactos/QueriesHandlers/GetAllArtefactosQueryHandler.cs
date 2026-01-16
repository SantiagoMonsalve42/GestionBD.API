using MediatR;
using GestionBD.Application.Contracts.Artefactos;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Artefactos.Queries;

namespace GestionBD.Application.Artefactos.QueriesHandlers;

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