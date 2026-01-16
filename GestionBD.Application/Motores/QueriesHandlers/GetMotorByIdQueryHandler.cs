using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Motores;
using GestionBD.Application.Motores.Queries;
using MediatR;

namespace GestionBD.Application.Motores.QueriesHandlers;

public sealed class GetMotorByIdQueryHandler : IRequestHandler<GetMotorByIdQuery, MotorResponse?>
{
    private readonly IMotorReadRepository _repository;

    public GetMotorByIdQueryHandler(IMotorReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<MotorResponse?> Handle(GetMotorByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.IdMotor, cancellationToken);
    }
}