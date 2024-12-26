using Domain.Models;
using Domain.Repositories;
using SharedMessages;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;

    public OrderRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    public async Task<Order> SaveOrderAsync(Order order)
    {
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();
        var result = await _dbContext.Orders.FindAsync(order.OrderId);
        if (result == null)
        {
            throw new Exception("Order not saved or found");
        }
        return result;
    }

    public Task<CancelOrder> CancelOrder()
    {
        //TODO: Implement - updates the order with reason for order not completed.
        throw new NotImplementedException();
    }
}

