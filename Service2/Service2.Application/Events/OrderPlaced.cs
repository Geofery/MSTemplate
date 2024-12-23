using NServiceBus;

namespace Application.Events;

    public class OrderPlaced : IEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
    }


