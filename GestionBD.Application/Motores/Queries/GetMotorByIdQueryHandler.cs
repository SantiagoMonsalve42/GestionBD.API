using GestionBD.Application.Abstractions.Readers;
using GestionBD.Application.Contracts.Motores;
using MediatR;

namespace GestionBD.Application.Motores.Queries;

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