using NServiceBus;

namespace Application.Commands
{
    public class ProcessPayment : ICommand
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
