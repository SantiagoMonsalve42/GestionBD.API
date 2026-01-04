using MediatR;
using GestionBD.Application.Abstractions;
using GestionBD.Application.Contracts.Motores;

namespace GestionBD.Application.Motores.Queries;

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