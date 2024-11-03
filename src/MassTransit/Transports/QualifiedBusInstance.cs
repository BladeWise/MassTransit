namespace MassTransit.Transports
{
    using System;
    using MassTransit.Configuration;

    public class QualifiedBusInstance : IQualifiedBusInstance, IBusInstance<IBus>
    {
        public IBus Bus => BusInstance.Bus;

        public IBusControl BusControl => BusInstance.BusControl;

        public IBusInstance BusInstance { get; }

        public IHostConfiguration HostConfiguration => BusInstance.HostConfiguration;

        public Type InstanceType => typeof(IBus);

        public object Qualifier { get; }

        public string Name { get; }

        public QualifiedBusInstance(IBusInstance instance, object qualifier)
        {
            BusInstance = instance;
            Qualifier = qualifier;
            Name = FormatBusName(qualifier);
        }

        public void Connect<TRider>(IRiderControl riderControl) where TRider : IRider => BusInstance.Connect<TRider>(riderControl);

        public TRider GetRider<TRider>() where TRider : IRider => BusInstance.GetRider<TRider>();

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null) => BusInstance.ConnectReceiveEndpoint(definition, endpointNameFormatter, configure);

        public HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null) => BusInstance.ConnectReceiveEndpoint(queueName, configure);

        static string FormatBusName(object qualifier)
        {
            return qualifier is null or "" ? "masstransit-bus" : $"masstransit-bus.{qualifier}";
        }
    }

    public class QualifiedBusInstance<TBus> : MultiBusInstance<TBus>, IQualifiedBusInstance where TBus : IBus
    {
        public object Qualifier { get; }

        public QualifiedBusInstance(TBus bus, IBusInstance instance, object qualifier) : base(bus, instance)
        {
            Qualifier = qualifier;
            Name = FormatBusName(qualifier);
        }

        static string FormatBusName(object qualifier)
        {
            var name = typeof(TBus).Name;
            if (name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]))
                name = name.Substring(1);

            return qualifier is null or "" ? $"masstransit-{KebabCaseEndpointNameFormatter.Instance.SanitizeName(name)}" : $"masstransit-{KebabCaseEndpointNameFormatter.Instance.SanitizeName(name)}.{qualifier}";
        }
    }
}
