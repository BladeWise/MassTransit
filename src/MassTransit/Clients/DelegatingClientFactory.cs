namespace MassTransit.Clients
{
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using MassTransit.Transports;

    public class DelegatingClientFactory<TBus>(IServiceProvider serviceProvider, Func<IServiceProvider, TBus, IClientFactory> clientFactoryBuilder) : IClientFactory where TBus : IBus
    {
        readonly ConcurrentDictionary<TBus, Lazy<IClientFactory>> _clientFactories = new();

        public IClientFactory CurrentClientFactory =>
                _clientFactories.GetOrAdd(serviceProvider.GetRequiredService<IBusInstanceResolver<TBus>>()
                                                         .GetBusInstance()
                                                         .Bus,
                                          bus => new(() => clientFactoryBuilder(serviceProvider, bus)))
                                .Value;

        public ClientFactoryContext Context => CurrentClientFactory.Context;

        public RequestHandle<T> CreateRequest<T>(T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest(message, cancellationToken, timeout);

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest(destinationAddress, message, cancellationToken, timeout);

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest(consumeContext, message, cancellationToken, timeout);

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, T message, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest(consumeContext, destinationAddress, message, cancellationToken, timeout);

        public RequestHandle<T> CreateRequest<T>(object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest<T>(values, cancellationToken, timeout);

        public RequestHandle<T> CreateRequest<T>(Uri destinationAddress, object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest<T>(destinationAddress, values, cancellationToken, timeout);

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest<T>(consumeContext, values, cancellationToken, timeout);

        public RequestHandle<T> CreateRequest<T>(ConsumeContext consumeContext, Uri destinationAddress, object values, CancellationToken cancellationToken = default, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequest<T>(consumeContext, destinationAddress, values, cancellationToken, timeout);

        public IRequestClient<T> CreateRequestClient<T>(RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequestClient<T>(timeout);

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequestClient<T>(consumeContext, timeout);

        public IRequestClient<T> CreateRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequestClient<T>(destinationAddress, timeout);

        public IRequestClient<T> CreateRequestClient<T>(ConsumeContext consumeContext, Uri destinationAddress, RequestTimeout timeout = default) where T : class => CurrentClientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout);
    }
}
