using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Instancias;

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