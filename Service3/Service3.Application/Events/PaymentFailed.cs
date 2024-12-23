using NServiceBus
    ;
namespace Application.Events
{
    public class PaymentFailed : IEvent
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }
}

