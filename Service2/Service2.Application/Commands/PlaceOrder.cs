using NServiceBus;

namespace Application.Commands
{
    public class PlaceOrder : ICommand
    {
        public Guid UserId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
