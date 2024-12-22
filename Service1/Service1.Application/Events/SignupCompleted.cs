namespace Application.Events
{
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

