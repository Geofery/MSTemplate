using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using SharedMessages;

namespace Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(AppDbContext dbContext, ILogger<OrderRepository> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger;
    }
    public async Task<Order> SaveOrderAsync(Order order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order), "Order cannot be null.");
        }

        try
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Order saved successfully. OrderId: {OrderId}", order.OrderId);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save the order. OrderId: {OrderId}", order.OrderId);
            throw new Exception("An error occurred while saving the order.", ex);
        }
    }

    public async Task<Order> CancelOrderAsync(CancelOrder cancelOrder)
    {
        if (cancelOrder == null)
        {
            throw new ArgumentNullException(nameof(cancelOrder), "CancelOrder cannot be null.");
        }

        try
        {
            var order = await _dbContext.Orders.FindAsync(cancelOrder.OrderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {cancelOrder.OrderId} not found.");
            }

            order.PaymentId = cancelOrder.PaymentId;
            order.Reason = cancelOrder.Reason;

            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Order cancelled successfully. OrderId: {OrderId}, Reason: {Reason}",
                cancelOrder.OrderId, cancelOrder.Reason);

            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel the order. OrderId: {OrderId}", cancelOrder.OrderId);
            throw new Exception("An error occurred while canceling the order.", ex);
        }
    }
}

