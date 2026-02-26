using GestionBD.Application.Contracts.Parametros;
using MediatR;

namespace GestionBD.Application.Parametros.Queries;

public sealed record GetAllParametrosQuery : IRequest<IEnumerable<ParametroResponse>>;