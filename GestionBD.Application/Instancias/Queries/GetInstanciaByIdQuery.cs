using MediatR;
using GestionBD.Application.Contracts.Instancias;

namespace GestionBD.Application.Instancias.Queries;

public sealed record GetInstanciaByIdQuery(decimal IdInstancia) : IRequest<InstanciaResponse?>;