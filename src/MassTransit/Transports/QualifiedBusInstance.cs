namespace MassTransit.Transports
{
    using System;
    using MassTransit.Configuration;

    public class QualifiedBusInstance(IBusInstance instance, object qualifier, string name) : IQualifiedBusInstance, IBusInstance<IBus>
    {
        public IBus Bus => BusInstance.Bus;

        public IBusControl BusControl => BusInstance.BusControl;

        public IBusInstance BusInstance { get; } = instance;

        public IHostConfiguration HostConfiguration => BusInstance.HostConfiguration;

        public Type InstanceType => typeof(IBus);

        public object Qualifier { get; } = qualifier;

        public string Name { get; } = name;

        public void Connect<TRider>(IRiderControl riderControl) where TRider : IRider => BusInstance.Connect<TRider>(riderControl);

        public TRider GetRider<TRider>() where TRider : IRider => BusInstance.GetRider<TRider>();

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null) => BusInstance.ConnectReceiveEndpoint(definition, endpointNameFormatter, configure);

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null) => BusInstance.ConnectReceiveEndpoint(queueName, configure);
    }

    public class QualifiedBusInstance<TBus> : MultiBusInstance<TBus>, IQualifiedBusInstance where TBus : IBus
    {
        public object Qualifier { get; }

        public QualifiedBusInstance(TBus bus, IBusInstance instance, object qualifier, string name) : base(bus, instance)
        {
            Qualifier = qualifier;
            Name = name;
        }
    }
}
