using MediatR;
using GestionBD.Application.Contracts.Parametros;

namespace GestionBD.Application.Parametros.Queries;

public sealed record GetAllParametrosQuery : IRequest<IEnumerable<ParametroResponse>>;