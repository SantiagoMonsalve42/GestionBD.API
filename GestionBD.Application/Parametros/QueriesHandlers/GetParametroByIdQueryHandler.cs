using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Parametros.Queries;
using MediatR;

namespace GestionBD.Application.Parametros.QueriesHandlers;

public sealed class GetParametroByIdQueryHandler : IRequestHandler<GetParametroByIdQuery, ParametroResponse?>
{
    private readonly IParametroReadRepository _repository;

    public GetParametroByIdQueryHandler(IParametroReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<ParametroResponse?> Handle(GetParametroByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdParametro, cancellationToken);
    }
}