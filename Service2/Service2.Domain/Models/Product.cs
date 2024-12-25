using System;

namespace Domain.Models
{
    public class Product
    {
        private Product() { } // Parameterless constructor for EF Core

        public Product(Guid productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        // Foreign key for the Order
        public Guid OrderId { get; set; }
        public Order Order { get; set; } // Navigation property
    }
}
