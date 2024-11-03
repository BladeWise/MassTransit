namespace MassTransit.Transports
{
    using MassTransit.Configuration;
    using MassTransit.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    public class QualifiedBusInstanceSupportProvider<TBusFactory> : IBusInstanceProvider<IBus>, IBusInstanceResolver<IBus>, IBindBusReceiveEndpointConnectorProvider<IBus>, IBindBusReceiveEndpointConnectorResolver<IBus>
        where TBusFactory : IRegistrationBusFactory
    {
        const string DefaultQualifier = "";

        static string FormatBusName(object qualifier)
        {
            if (qualifier is null or DefaultQualifier)
                return string.Empty; // Use the default

            return qualifier as string ?? qualifier.ToString();
        }

        static QualifiedBusInstance CreateBus(TBusFactory busFactory, IServiceProvider provider, object qualifier)
        {
            var specifications = provider.GetServices<Bind<IBus, IBusInstanceSpecification>>()
                                         .Select(x => x.Value);
            var context = provider.GetRequiredService<Bind<IBus, IBusRegistrationContext>>()
                                  .Value;

            var busName = FormatBusName(qualifier);
            var instance = busFactory.CreateBus(context, specifications, busName);

            return new(instance, qualifier);
        }

        readonly ConcurrentDictionary<object, Lazy<BusInstanceInfo>> _register = new();
        readonly IServiceProvider _serviceProvider;
        readonly TBusFactory _busFactory;

        public QualifiedBusInstanceSupportProvider(IServiceProvider serviceProvider, TBusFactory busFactory)
        {
            _serviceProvider = serviceProvider;
            _busFactory = busFactory;
        }

        public IEnumerable<Bind<IBus, IReceiveEndpointConnector>> GetBinds()
        {
            var busInstanceQualifierProvider = _serviceProvider.GetService<IBusInstanceQualifierProvider>();
            foreach (var qualifier in busInstanceQualifierProvider?.GetBusInstanceQualifiers(typeof(IBus)))
            {
                yield return GetBusInstanceInfo(qualifier)
                    .Bind;
            }
        }

        public Bind<IBus, IReceiveEndpointConnector> GetBind()
        {
            var busInstanceQualifierResolver = _serviceProvider.GetService<IBusInstanceQualifierResolver>();
            var qualifier = (busInstanceQualifierResolver?.GetBusInstanceQualifier(typeof(IBus)) ?? DefaultQualifier) ?? throw new InvalidOperationException("No qualifier available in current scope.");
            return GetBusInstanceInfo(qualifier)
                .Bind;
        }

        public IEnumerable<IBusInstance<IBus>> GetBusInstances()
        {
            var busInstanceQualifierProvider = _serviceProvider.GetService<IBusInstanceQualifierProvider>();
            foreach (var qualifier in busInstanceQualifierProvider?.GetBusInstanceQualifiers(typeof(IBus)) ?? [DefaultQualifier])
            {
                yield return GetBusInstanceInfo(qualifier)
                    .BusInstance;
            }
        }

        IEnumerable<IBusInstance> IBusInstanceProvider.GetBusInstances() => GetBusInstances();

        public IBusInstance<IBus> GetBusInstance()
        {
            var busInstanceQualifierResolver = _serviceProvider.GetService<IBusInstanceQualifierResolver>();
            var qualifier = (busInstanceQualifierResolver?.GetBusInstanceQualifier(typeof(IBus)) ?? DefaultQualifier) ?? throw new InvalidOperationException("No qualifier available in current scope.");
            return GetBusInstanceInfo(qualifier)
                .BusInstance;
        }

        IBusInstance IBusInstanceResolver.GetBusInstance() => GetBusInstance();

        BusInstanceInfo GetBusInstanceInfo(object qualifier) =>
            _register.GetOrAdd(qualifier,
                         k =>
                         {
                             return new(() =>
                             {
                                 var busInstance = CreateBus(_busFactory, _serviceProvider, k);
                                 var bind = Bind<IBus>.Create<IReceiveEndpointConnector>(busInstance);
                                 return new(busInstance, bind);
                             });
                         })
                     .Value;


        class BusInstanceInfo
        {
            public IBusInstance<IBus> BusInstance { get; }
            public Bind<IBus, IReceiveEndpointConnector> Bind { get; }

            public BusInstanceInfo(IBusInstance<IBus> busInstance, Bind<IBus, IReceiveEndpointConnector> bind)
            {
                BusInstance = busInstance;
                Bind = bind;
            }
        }
    }

    public class QualifiedBusInstanceSupportProvider<TBusFactory, TBus, TBusInstance> : IBusInstanceProvider<TBus>, IBusInstanceResolver<TBus>, IBindBusReceiveEndpointConnectorProvider<TBus>, IBindBusReceiveEndpointConnectorResolver<TBus> where TBus : class, IBus
        where TBusFactory : IRegistrationBusFactory
        where TBusInstance : BusInstance<TBus>, TBus
    {
        const string DefaultQualifier = "";

        static string FormatBusName(object qualifier)
        {
            var name = typeof(TBus).Name;
            return (qualifier is null or "") ? name : $"{name}.{qualifier}";
        }

        static QualifiedBusInstance<TBus> CreateBus(TBusFactory busFactory, IServiceProvider provider, object qualifier)
        {
            var specifications = provider.GetServices<Bind<TBus, IBusInstanceSpecification>>()
                                         .Select(x => x.Value);
            var context = provider.GetRequiredService<Bind<TBus, IBusRegistrationContext>>()
                                  .Value;
            var busName = FormatBusName(qualifier);
            var instance = busFactory.CreateBus(context, specifications, busName);
            var busInstance = provider.GetService<TBusInstance>() ?? ActivatorUtilities.CreateInstance<TBusInstance>(provider, instance.BusControl);

            return new(busInstance, instance, qualifier);
        }

        readonly ConcurrentDictionary<object, Lazy<BusInstanceInfo>> _register = new();
        readonly IServiceProvider _serviceProvider;
        readonly TBusFactory _busFactory;

        public QualifiedBusInstanceSupportProvider(IServiceProvider serviceProvider, TBusFactory busFactory)
        {
            _serviceProvider = serviceProvider;
            _busFactory = busFactory;
        }

        public IEnumerable<Bind<TBus, IReceiveEndpointConnector>> GetBinds()
        {
            var busInstanceQualifierProvider = _serviceProvider.GetService<IBusInstanceQualifierProvider>();
            foreach (var qualifier in busInstanceQualifierProvider?.GetBusInstanceQualifiers(typeof(TBus)))
            {
                yield return GetBusInstanceInfo(qualifier)
                    .Bind;
            }
        }

        public Bind<TBus, IReceiveEndpointConnector> GetBind()
        {
            var busInstanceQualifierResolver = _serviceProvider.GetService<IBusInstanceQualifierResolver>();
            var qualifier = busInstanceQualifierResolver?.GetBusInstanceQualifier(typeof(TBus)) ?? DefaultQualifier;
            return GetBusInstanceInfo(qualifier)
                .Bind;
        }

        public IEnumerable<IBusInstance<TBus>> GetBusInstances()
        {
            var busInstanceQualifierProvider = _serviceProvider.GetService<IBusInstanceQualifierProvider>();
            foreach (var qualifier in busInstanceQualifierProvider?.GetBusInstanceQualifiers(typeof(TBus)) ?? [DefaultQualifier])
            {
                yield return GetBusInstanceInfo(qualifier)
                    .BusInstance;
            }
        }

        IEnumerable<IBusInstance> IBusInstanceProvider.GetBusInstances() => GetBusInstances();

        public IBusInstance<TBus> GetBusInstance()
        {
            var busInstanceQualifierResolver = _serviceProvider.GetService<IBusInstanceQualifierResolver>();
            var qualifier = busInstanceQualifierResolver?.GetBusInstanceQualifier(typeof(TBus)) ?? DefaultQualifier;
            return GetBusInstanceInfo(qualifier)
                .BusInstance;
        }

        IBusInstance IBusInstanceResolver.GetBusInstance() => GetBusInstance();

        BusInstanceInfo GetBusInstanceInfo(object qualifier) =>
            _register.GetOrAdd(qualifier,
                         k =>
                         {
                             return new(() =>
                             {
                                 var busInstance = CreateBus(_busFactory, _serviceProvider, k);
                                 var bind = Bind<TBus>.Create<IReceiveEndpointConnector>(busInstance);
                                 return new(busInstance, bind);
                             });
                         })
                     .Value;


        class BusInstanceInfo
        {
            public IBusInstance<TBus> BusInstance { get; }
            public Bind<TBus, IReceiveEndpointConnector> Bind { get; }

            public BusInstanceInfo(IBusInstance<TBus> busInstance, Bind<TBus, IReceiveEndpointConnector> bind)
            {
                BusInstance = busInstance;
                Bind = bind;
            }
        }
    }
}
