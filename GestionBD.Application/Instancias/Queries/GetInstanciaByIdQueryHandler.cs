using MediatR;
using GestionBD.Application.Contracts.Instancias;
using GestionBD.Application.Abstractions.Readers;

namespace GestionBD.Application.Instancias.Queries;

public sealed class GetInstanciaByIdQueryHandler : IRequestHandler<GetInstanciaByIdQuery, InstanciaResponse?>
{
    private readonly IInstanciaReadRepository _repository;

    public GetInstanciaByIdQueryHandler(IInstanciaReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<InstanciaResponse?> Handle(GetInstanciaByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdInstancia, cancellationToken);
    }
}