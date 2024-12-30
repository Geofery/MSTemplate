namespace Domain.Models
{
    public class Order
    {
        private Order() { } // Parameterless constructor for EF Core

        public Order(Guid orderId, Guid userId, ICollection<Product> products)
        {
            OrderId = orderId;
            UserId = userId;
            Products = products;
        }

        public Order(Guid orderId, Guid userId, ICollection<Product> products, Guid paymentId, string status, string reason)
        {
            OrderId = orderId;
            UserId = userId;
            Products = products;
            PaymentId = paymentId;
            Status = status;
            Reason = reason;
        }

        public Order(Guid orderId, Guid paymentId, string status, string reason, decimal amount)
        {
            OrderId = orderId;
            PaymentId = paymentId;
            Status = status;
            Reason = reason;
            Amount = amount;
        }

        public Order(Guid orderId, Guid userId, ICollection<Product> products, string status, string reason)
        {
            OrderId = orderId;
            UserId = userId;
            Products = products;
            Status = status;
            Reason = reason;
        }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; } // Could be linked to a User entity in the future
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public Guid PaymentId { get; set; }
        public string Status { get; set; } // "Processed" or "Failed"
        public string Reason { get; set; }
        public decimal Amount { get; set; }
    }
}
