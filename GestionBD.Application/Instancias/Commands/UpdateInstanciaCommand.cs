using MediatR;
using GestionBD.Application.Contracts.Instancias;

namespace GestionBD.Application.Instancias.Commands;

public sealed record UpdateInstanciaCommand(UpdateInstanciaRequest Request) : IRequest<Unit>;