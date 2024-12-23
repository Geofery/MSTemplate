using System;
using NServiceBus;
namespace Application.Events
{
    public class UserValidated : IEvent
    {
        public Guid UserId { get; set; }
    }
}

