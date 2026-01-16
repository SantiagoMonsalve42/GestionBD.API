using MediatR;
using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Motores.Queries;

namespace GestionBD.Application.Motores.QueriesHandlers;

public sealed class GetAllMotoresQueryHandler : IRequestHandler<GetAllMotoresQuery, IEnumerable<MotorResponse>>
{
    private readonly IMotorReadRepository _repository;

    public GetAllMotoresQueryHandler(IMotorReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MotorResponse>> Handle(GetAllMotoresQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}