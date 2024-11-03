namespace MassTransit.Transports
{
    using MassTransit.DependencyInjection;

    public interface IBindBusReceiveEndpointConnectorResolver<TBus>
        where TBus : IBus
    {
        Bind<TBus, IReceiveEndpointConnector> GetBind();
    }
}
