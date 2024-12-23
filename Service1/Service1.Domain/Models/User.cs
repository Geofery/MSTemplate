using Domain.Models;

namespace Domain.Models;

public class User
{

    public User(Guid leadId, string name, string email, string password, Address address) {
        LeadId = leadId;
        Name = name;
        Email = email;
        Password = password;
        Address = address;
    }

    public Guid LeadId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Address Address { get; set; }
}
