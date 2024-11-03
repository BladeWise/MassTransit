#nullable enable
namespace MassTransit.SqlTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Configuration;
    using Transports;


    public class SqlRegistrationBusFactory :
        TransportRegistrationBusFactory<ISqlReceiveEndpointConfigurator>
    {
        readonly Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? _configure;

        public SqlRegistrationBusFactory(Action<IBusRegistrationContext, ISqlBusFactoryConfigurator>? configure)
        {
            _configure = configure;
        }

        public override IBusInstance CreateBus(IBusRegistrationContext context, IEnumerable<IBusInstanceSpecification> specifications, string busName)
        {
            var busConfiguration = new SqlBusConfiguration(new SqlTopologyConfiguration(SqlBusFactory.CreateMessageTopology()));
            var configurator = new SqlBusFactoryConfigurator(busConfiguration);

            configurator.UseRawJsonSerializer(RawSerializerOptions.CopyHeaders, true);

            // var options = context.GetRequiredService<IOptionsMonitor<DbTransportOptions>>().Get(busName);
            //
            // configurator.Host(options.Host, options.Port, options.VHost, h =>
            // {
            //     if (!string.IsNullOrWhiteSpace(options.User))
            //         h.Username(options.User);
            //
            //     if (!string.IsNullOrWhiteSpace(options.Pass))
            //         h.Password(options.Pass);
            // });

            return CreateBus(busConfiguration.HostConfiguration, configurator, context, _configure, specifications);
        }
    }
}
