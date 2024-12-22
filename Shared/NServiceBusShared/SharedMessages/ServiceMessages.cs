﻿namespace SharedMessages
{
    public class MessageFromService1 : IEvent
    {
        public required string Content { get; set; }
    }

    public class MessageFromService2 : IMessage
    {
        public required string Content { get; set; }
    }

    public class UserCreated: IEvent
    {
        public Guid LeadId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}