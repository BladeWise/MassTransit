namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using MassTransit.DependencyInjection;

    public interface IBindBusReceiveEndpointConnectorProvider<TBus>
        where TBus : IBus
    {
        IEnumerable<Bind<TBus, IReceiveEndpointConnector>> GetBinds();
    }
}
