namespace MassTransit.Transports
{
    using System.Collections.Generic;

    public interface IBusInstanceProvider
    {
        IEnumerable<IBusInstance> GetBusInstances();
    }
    
    public interface IBusInstanceProvider<out TBus> : IBusInstanceProvider
        where TBus : IBus
    {
        new IEnumerable<IBusInstance<TBus>> GetBusInstances();
    }
}
