using GestionBD.Application.Contracts.Instancias;
using MediatR;

namespace GestionBD.Application.Instancias.Commands;

public sealed record CreateInstanciaCommand(CreateInstanciaRequest Request) : IRequest<decimal>;