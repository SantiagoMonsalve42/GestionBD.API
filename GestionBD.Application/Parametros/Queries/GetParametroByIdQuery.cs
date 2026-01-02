using MediatR;
using GestionBD.Application.Contracts.Parametros;

namespace GestionBD.Application.Parametros.Queries;

public sealed record GetParametroByIdQuery(decimal IdParametro) : IRequest<ParametroResponse?>;