using GestionBD.Application.Contracts.Instancias;
using MediatR;

namespace GestionBD.Application.Instancias.Queries;

public sealed record GetInstanciaByIdQuery(decimal IdInstancia) : IRequest<InstanciaResponse?>;