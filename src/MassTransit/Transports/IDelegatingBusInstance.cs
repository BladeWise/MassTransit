namespace MassTransit.Transports
{
    public interface IDelegatingBusInstance : IBusInstance
    {
        IBusInstance CurrentBusInstance { get; }
    }


    public interface IDelegatingBusInstance<out TBus> : IBusInstance<TBus>, IDelegatingBusInstance where TBus : IBus
    {
        new IBusInstance<TBus> CurrentBusInstance { get; }
    }
}
