namespace SharedMessages
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

    public class SignupCompleted : IEvent
    {
        public SignupCompleted(Guid leadId, string name, string email, string password)
        {
            LeadId = leadId;
            Name = name;
            Email = email;
            Password = password;
        }

        public Guid LeadId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
