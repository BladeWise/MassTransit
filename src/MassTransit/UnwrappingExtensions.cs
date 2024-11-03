namespace MassTransit
{
    using MassTransit.Transports;


    public static class UnwrappingExtensions
    {
        public static IBusInstance Unwrap(this IBusInstance busInstance)
        {
            var current = busInstance;
            while (true)
            {
                if (current is IDelegatingBusInstance d)
                    current = d.CurrentBusInstance;
                else if (current is IWrappingBusInstance w)
                    current = w.BusInstance;
                else
                    return current;
            }
        }
    }
}
