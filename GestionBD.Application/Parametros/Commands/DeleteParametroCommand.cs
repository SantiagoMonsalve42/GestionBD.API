using MediatR;

namespace GestionBD.Application.Parametros.Commands;

public sealed record DeleteParametroCommand(decimal IdParametro) : IRequest<Unit>;