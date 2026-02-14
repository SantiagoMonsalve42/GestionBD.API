using GestionBD.Application.Abstractions.Repositories.Query;
using GestionBD.Application.Contracts.Entregables;
using GestionBD.Application.Entregables.Queries;
using MediatR;

namespace GestionBD.Application.Entregables.QueriesHandlers
{
    public sealed class GetEntregablesRevisionQueryHandler : IRequestHandler<GetEntregablesRevisionQuery, IEnumerable<EntregableRevisionResponse>>
    {
        private readonly IEntregableReadRepository _repository;

        public GetEntregablesRevisionQueryHandler(IEntregableReadRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<EntregableRevisionResponse>> Handle(GetEntregablesRevisionQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllRevisionesAsync(cancellationToken);
        }
    }
}
