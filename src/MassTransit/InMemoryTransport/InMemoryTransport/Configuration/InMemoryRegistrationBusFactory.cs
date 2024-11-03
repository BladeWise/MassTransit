namespace MassTransit.InMemoryTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Transports;


    public class InMemoryRegistrationBusFactory :
        TransportRegistrationBusFactory<IInMemoryReceiveEndpointConfigurator>
    {
        readonly Uri _baseAddress;
        readonly Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> _configure;

        public InMemoryRegistrationBusFactory(Uri baseAddress, Action<IBusRegistrationContext, IInMemoryBusFactoryConfigurator> configure)
        {
            _baseAddress = baseAddress;
            _configure = configure;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var busConfiguration = new InMemoryBusConfiguration(new InMemoryTopologyConfiguration(InMemoryBus.CreateMessageTopology()), _baseAddress);
            var configurator = new InMemoryBusFactoryConfigurator(busConfiguration);

            return CreateBus(busConfiguration.HostConfiguration, configurator, context, _configure, specifications);
        }
    }
}
