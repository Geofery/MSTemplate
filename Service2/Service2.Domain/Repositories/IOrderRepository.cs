using System;
using Domain.Models;

namespace Domain.Repositories
{
	public interface IOrderRepository
	{
		Task<Order> SaveOrderAsync(Order order);
	}
}

