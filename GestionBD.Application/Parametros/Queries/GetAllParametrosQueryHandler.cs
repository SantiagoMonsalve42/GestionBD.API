using MediatR;
using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Abstractions.Repositories.Query;

namespace GestionBD.Application.Parametros.Queries;

public sealed class GetAllParametrosQueryHandler : IRequestHandler<GetAllParametrosQuery, IEnumerable<ParametroResponse>>
{
    private readonly IParametroReadRepository _repository;

    public GetAllParametrosQueryHandler(IParametroReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ParametroResponse>> Handle(GetAllParametrosQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}