using System;
namespace Domain.Repositories
{
	public interface IOrderRepository
	{
		Task SaveOrderAsync(Order order);
	}
}

