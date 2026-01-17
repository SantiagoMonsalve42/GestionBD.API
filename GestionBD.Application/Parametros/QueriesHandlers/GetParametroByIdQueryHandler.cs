using MediatR;
using GestionBD.Application.Contracts.Parametros;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Parametros.Queries;

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