using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Entregables;
using MediatR;

namespace GestionBD.Application.Entregables.Queries
{
    public sealed class GetAllEntregablesByEjecucionIdQueryHandler : IRequestHandler<GetAllEntregablesByEjecucionIdQuery, IEnumerable<EntregableResponseEstado>>
    {
        private readonly IEntregableReadRepository _repository;

        public GetAllEntregablesByEjecucionIdQueryHandler(IEntregableReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EntregableResponseEstado>> Handle(GetAllEntregablesByEjecucionIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllByIdEjecucionAsync(request.IdEjecucion, cancellationToken);
        }
    }
}
