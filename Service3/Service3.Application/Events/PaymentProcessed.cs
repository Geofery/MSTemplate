using NServiceBus;

namespace Application.Events
{
    public class PaymentProcessed : IEvent
    {
        public Guid OrderId { get; set; }
    }
}

