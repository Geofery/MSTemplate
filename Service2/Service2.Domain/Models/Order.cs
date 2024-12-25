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

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; } // Could be linked to a User entity in the future
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
