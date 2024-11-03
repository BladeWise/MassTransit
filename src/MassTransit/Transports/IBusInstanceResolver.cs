namespace MassTransit.Transports
{
    public interface IBusInstanceResolver
    {
        IBusInstance GetBusInstance();
    }
    
    public interface IBusInstanceResolver<out TBus> : IBusInstanceResolver where TBus : IBus
    {
        new IBusInstance<TBus> GetBusInstance();
    }
}
