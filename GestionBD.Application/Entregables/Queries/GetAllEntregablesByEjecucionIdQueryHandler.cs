using GestionBD.Application.Abstractions.Readers;
using GestionBD.Application.Contracts.Entregables;
using MediatR;

namespace GestionBD.Application.Entregables.Queries
{
    public sealed class GetAllEntregablesByEjecucionIdQueryHandler : IRequestHandler<GetAllEntregablesByEjecucionIdQuery, IEnumerable<EntregableResponse>>
    {
        private readonly IEntregableReadRepository _repository;

        public GetAllEntregablesByEjecucionIdQueryHandler(IEntregableReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EntregableResponse>> Handle(GetAllEntregablesByEjecucionIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllByIdEjecucionAsync(request.IdEjecucion, cancellationToken);
        }
    }
}
