using MediatR;
public sealed record TestConnectionCommand(decimal IdInstancia) : IRequest<bool>;