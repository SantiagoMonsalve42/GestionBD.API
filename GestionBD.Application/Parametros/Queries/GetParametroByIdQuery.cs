using GestionBD.Application.Contracts.Parametros;
using MediatR;

namespace GestionBD.Application.Parametros.Queries;

public sealed record GetParametroByIdQuery(decimal IdParametro) : IRequest<ParametroResponse?>;