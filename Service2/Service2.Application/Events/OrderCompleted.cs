using NServiceBus;

namespace Application.Events
{
    public class OrderCompleted : IEvent
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
    }
}

