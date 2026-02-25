using MediatR;

namespace GestionBD.UnitTests.API.Helpers;

public sealed class TestMediator : IMediator
{
    private readonly Dictionary<Type, Func<object, object?>> _handlers = new();

    public void Register<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        where TRequest : IRequest<TResponse>
    {
        _handlers[typeof(TRequest)] = request => handler((TRequest)request);
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        if (!_handlers.TryGetValue(request.GetType(), out var handler))
            throw new InvalidOperationException($"No handler registered for {request.GetType().Name}.");

        return Task.FromResult((TResponse)handler(request)!);
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        if (!_handlers.TryGetValue(request.GetType(), out var handler))
            throw new InvalidOperationException($"No handler registered for {request.GetType().Name}.");

        _ = handler(request);
        return Task.CompletedTask;
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        if (!_handlers.TryGetValue(request.GetType(), out var handler))
            throw new InvalidOperationException($"No handler registered for {request.GetType().Name}.");

        return Task.FromResult(handler(request));
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        throw new NotSupportedException("Stream requests are not supported in TestMediator.");
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Stream requests are not supported in TestMediator.");
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification => Task.CompletedTask;
}