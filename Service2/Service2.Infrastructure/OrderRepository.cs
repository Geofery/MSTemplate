using Domain;
using Domain.Repositories;

namespace Infrastructure;

public class OrderRepository : IOrderRepository
{
    public Task SaveOrderAsync(Order order)
    {
        //TODO Simulate saving the order.
        return Task.CompletedTask;
    }
}
