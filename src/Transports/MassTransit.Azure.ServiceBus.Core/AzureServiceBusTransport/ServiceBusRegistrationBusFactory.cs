namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using MassTransit.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Transports;


    public class ServiceBusRegistrationBusFactory :
        TransportRegistrationBusFactory<IServiceBusReceiveEndpointConfigurator>
    {
        readonly Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator> _configure;

        public ServiceBusRegistrationBusFactory(Action<IBusRegistrationContext, IServiceBusBusFactoryConfigurator> configure)
        {
            _configure = configure;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var busConfiguration = new ServiceBusBusConfiguration(new ServiceBusTopologyConfiguration(AzureBusFactory.CreateMessageTopology()));
            var configurator = new ServiceBusBusFactoryConfigurator(busConfiguration);

            var options = context.GetRequiredService<IOptionsMonitor<AzureServiceBusTransportOptions>>().Get(busName);
            if (!string.IsNullOrWhiteSpace(options.ConnectionString))
                configurator.Host(options.ConnectionString);

            return CreateBus(busConfiguration.HostConfiguration, configurator, context, _configure, specifications);
        }

        protected override IBusInstance CreateBusInstance(IBusControl bus, IHost<IServiceBusReceiveEndpointConfigurator> host,
            IHostConfiguration hostConfiguration, IBusRegistrationContext context)
        {
            return new ServiceBusInstance(bus, host, hostConfiguration, context);
        }
    }
}
