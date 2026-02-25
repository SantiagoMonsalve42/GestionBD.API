using GestionBD.Application.Contracts.Instancias;
using MediatR;

namespace GestionBD.Application.Instancias.Commands;

public sealed record UpdateInstanciaCommand(UpdateInstanciaRequest Request) : IRequest<Unit>;