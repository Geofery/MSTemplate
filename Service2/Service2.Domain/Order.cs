namespace Domain;

    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } // e.g., "Pending", "Completed"
    }

