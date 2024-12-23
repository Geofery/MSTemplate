using NServiceBus;

namespace Application.Events
{
    public class UserValidationFailed : IEvent
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; }
    }
}
