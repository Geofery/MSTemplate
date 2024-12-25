using System;
namespace Domain.Models
{
	public class Product
	{
		public Product(Guid productId, int quantity)
		{
			ProductId = productId;
			Quantity = quantity;
		}
		public Guid ProductId;
		public int Quantity;
	}
}

