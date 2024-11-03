namespace MassTransit
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Transports;


    public class DelegatingBusControl : DelegatingBus, IBusControl
    {
        public IBusControl CurrentBusControl => BusInstanceResolver.GetBusInstance().BusControl;

        public DelegatingBusControl(IBusInstanceResolver<IBus> busInstanceResolver) : base(busInstanceResolver)
        {
        }

        public Task<BusHandle> StartAsync(CancellationToken cancellationToken = default) => CurrentBusControl.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken = default) => CurrentBusControl.StopAsync(cancellationToken);

        public BusHealthResult CheckHealth() => CurrentBusControl.CheckHealth();
    }
}
