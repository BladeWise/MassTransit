namespace MassTransit.Transports
{
    public interface IQualifiedBusInstance : IBusInstance
    {
        object Qualifier { get; }
    }
}
