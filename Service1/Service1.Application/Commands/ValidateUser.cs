using Domain.Models;
using NServiceBus;

namespace Application.Commands
{
    public class ValidateUser : ICommand
    {
        public Guid UserId { get; set; }
        public Address Address { get; set; }
    }
}

