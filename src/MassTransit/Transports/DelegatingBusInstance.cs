namespace MassTransit.Transports
{
    using System;
    using MassTransit.Configuration;

    public class DelegatingBusInstance : IDelegatingBusInstance
    {
        readonly IBusInstanceResolver _busInstanceResolver;

        IBus IBusInstance.Bus => CurrentBusInstance.Bus;

        public IBusInstance CurrentBusInstance => _busInstanceResolver.GetBusInstance();

        public IBusControl BusControl => CurrentBusInstance.BusControl;

        public IHostConfiguration HostConfiguration => CurrentBusInstance.HostConfiguration;

        public Type InstanceType => CurrentBusInstance.InstanceType;

        public string Name => CurrentBusInstance.Name;

        public DelegatingBusInstance(IBusInstanceResolver busInstanceResolver)
        {
            _busInstanceResolver = busInstanceResolver;
        }

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null) => CurrentBusInstance.ConnectReceiveEndpoint(definition, endpointNameFormatter, configure);

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null) => CurrentBusInstance.ConnectReceiveEndpoint(queueName, configure);

        public void Connect<TRider>(IRiderControl riderControl) where TRider : IRider => CurrentBusInstance.Connect<TRider>(riderControl);

        public TRider GetRider<TRider>() where TRider : IRider => CurrentBusInstance.GetRider<TRider>();
    }

    public class DelegatingBusInstance<TBus>(IBusInstanceResolver<TBus> busInstanceResolver) : DelegatingBusInstance(busInstanceResolver), IDelegatingBusInstance<TBus> where TBus : IBus
    {
        IBus IBusInstance.Bus => CurrentBusInstance.Bus;

        public new IBusInstance<TBus> CurrentBusInstance => (IBusInstance<TBus>)base.CurrentBusInstance;

        public TBus Bus => CurrentBusInstance.Bus;

        public IBusInstance BusInstance => CurrentBusInstance.BusInstance;
    }
}
