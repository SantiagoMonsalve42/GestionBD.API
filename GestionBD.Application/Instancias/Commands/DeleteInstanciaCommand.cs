using MediatR;

namespace GestionBD.Application.Instancias.Commands;

public sealed record DeleteInstanciaCommand(decimal IdInstancia) : IRequest<Unit>;