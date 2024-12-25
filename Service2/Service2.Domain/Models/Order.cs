namespace Domain.Models;

    public class Order
    {
    public Order(Guid orderId, Guid userId, List<Product> products)
    {
        OrderId = orderId;
        UserId = userId;
        Products = products;

    }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public List<Product> Products { get; set; }
    }



