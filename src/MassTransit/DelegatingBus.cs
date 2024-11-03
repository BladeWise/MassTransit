namespace MassTransit
{
    using System;
    using System.Threading.Tasks;
    using System.Threading;
    using MassTransit.Transports;

    public class DelegatingBus : IBus
    {
        protected IBusInstanceResolver BusInstanceResolver { get; }
        public IBus CurrentBus => BusInstanceResolver.GetBusInstance().Bus;
        public Uri Address => CurrentBus.Address;
        public IBusTopology Topology => CurrentBus.Topology;

        public DelegatingBus(IBusInstanceResolver busInstanceResolver)
        {
            BusInstanceResolver = busInstanceResolver;
        }

        public ConnectHandle ConnectPublishObserver(IPublishObserver observer) => CurrentBus.ConnectPublishObserver(observer);

        public Task<ISendEndpoint> GetPublishSendEndpoint<T>() where T : class => CurrentBus.GetPublishSendEndpoint<T>();

        public Task Publish<T>(T message, CancellationToken cancellationToken = default) where T : class => CurrentBus.Publish(message, cancellationToken);

        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class => CurrentBus.Publish(message, publishPipe, cancellationToken);

        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class => CurrentBus.Publish(message, publishPipe, cancellationToken);

        public Task Publish(object message, CancellationToken cancellationToken = default) => CurrentBus.Publish(message, cancellationToken);

        public Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) => CurrentBus.Publish(message, publishPipe, cancellationToken);

        public Task Publish(object message, Type messageType, CancellationToken cancellationToken = default) => CurrentBus.Publish(message, messageType, cancellationToken);

        public Task Publish(object message, Type messageType, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) => CurrentBus.Publish(message, messageType, publishPipe, cancellationToken);

        public Task Publish<T>(object values, CancellationToken cancellationToken = default) where T : class => CurrentBus.Publish<T>(values, cancellationToken);

        public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = default) where T : class => CurrentBus.Publish(values, publishPipe, cancellationToken);

        public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = default) where T : class => CurrentBus.Publish<T>(values, publishPipe, cancellationToken);

        public ConnectHandle ConnectSendObserver(ISendObserver observer) => CurrentBus.ConnectSendObserver(observer);

        public Task<ISendEndpoint> GetSendEndpoint(Uri address) => CurrentBus.GetSendEndpoint(address);

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe) where T : class => CurrentBus.ConnectConsumePipe(pipe);

        public ConnectHandle ConnectConsumePipe<T>(IPipe<ConsumeContext<T>> pipe, ConnectPipeOptions options) where T : class => CurrentBus.ConnectConsumePipe(pipe, options);

        public ConnectHandle ConnectRequestPipe<T>(Guid requestId, IPipe<ConsumeContext<T>> pipe) where T : class => CurrentBus.ConnectRequestPipe(requestId, pipe);

        public ConnectHandle ConnectConsumeMessageObserver<T>(IConsumeMessageObserver<T> observer) where T : class => CurrentBus.ConnectConsumeMessageObserver(observer);

        public ConnectHandle ConnectConsumeObserver(IConsumeObserver observer) => CurrentBus.ConnectConsumeObserver(observer);

        public ConnectHandle ConnectReceiveObserver(IReceiveObserver observer) => CurrentBus.ConnectReceiveObserver(observer);

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer) => CurrentBus.ConnectReceiveEndpointObserver(observer);

        public ConnectHandle ConnectEndpointConfigurationObserver(IEndpointConfigurationObserver observer) => CurrentBus.ConnectEndpointConfigurationObserver(observer);

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null, Action<IReceiveEndpointConfigurator> configureEndpoint = null) => CurrentBus.ConnectReceiveEndpoint(definition, endpointNameFormatter, configureEndpoint);

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint = null) => CurrentBus.ConnectReceiveEndpoint(queueName, configureEndpoint);

        public void Probe(ProbeContext context) => CurrentBus.Probe(context);
    }

    public class DelegatingBus<TBus> : DelegatingBus where TBus : IBus
    {
        public DelegatingBus(IBusInstanceResolver<TBus> busInstanceResolver) : base(busInstanceResolver)
        {
        }
    }
}
