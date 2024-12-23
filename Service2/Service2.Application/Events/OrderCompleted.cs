using NServiceBus;

namespace Service2.Application.Events
{
    public class OrderCompleted : IEvent
    {
        public Guid OrderId { get; set; }
    }
}

