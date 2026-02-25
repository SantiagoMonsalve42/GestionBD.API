using GestionBD.Application.Contracts.Parametros;
using MediatR;

namespace GestionBD.Application.Parametros.Commands;

public sealed record CreateParametroCommand(CreateParametroRequest Request) : IRequest<decimal>;