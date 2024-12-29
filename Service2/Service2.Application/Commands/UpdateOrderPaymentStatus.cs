using System;
namespace Application.Commands
{
    public class UpdateOrderPaymentStatus : ICommand
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; } // Optional, for failed payments
        public decimal Amount { get; set; }
    }

}

