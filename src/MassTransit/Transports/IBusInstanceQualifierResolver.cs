namespace MassTransit.Transports
{
    using System;

    public interface IBusInstanceQualifierResolver
    {
        object GetBusInstanceQualifier(Type busType);
    }
}
