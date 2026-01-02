using MediatR;
using GestionBD.Application.Contracts.Parametros;

namespace GestionBD.Application.Parametros.Commands;

public sealed record UpdateParametroCommand(UpdateParametroRequest Request) : IRequest<Unit>;