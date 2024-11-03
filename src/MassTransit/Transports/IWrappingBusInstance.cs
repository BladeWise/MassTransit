namespace MassTransit.Transports
{
    public interface IWrappingBusInstance : IBusInstance
    {
        /// <summary>
        /// The original bus instance (i.e., the one wrapped in a multi-bus instance or a qualified instance)
        /// </summary>
        IBusInstance BusInstance { get; }
    }
}
