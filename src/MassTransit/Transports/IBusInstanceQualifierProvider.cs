namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;


    public interface IBusInstanceQualifierProvider
    {
        IEnumerable<object> GetBusInstanceQualifiers(Type busType);
    }
}
