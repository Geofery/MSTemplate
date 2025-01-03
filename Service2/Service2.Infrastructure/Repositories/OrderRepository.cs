﻿using System.Net.NetworkInformation;
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

    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync();
            _logger.LogInformation("OrderService Database connectivity check: {Status}", canConnect ? "Success" : "Failure");
            return canConnect;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connectivity health check failed.");
            return false;
        }
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order), "Order cannot be null.");
        }

        try
        {
            var existingOrder = await _dbContext.Orders.FindAsync(order.OrderId);

            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Order with ID {order.OrderId} not found.");
            }

            existingOrder.PaymentId = order.PaymentId;
            existingOrder.Status = order.Status;
            existingOrder.Amount = order.Amount;

            if (!string.IsNullOrEmpty(order.Reason))
            {
                existingOrder.Reason = order.Reason;
            }

            _dbContext.Orders.Update(existingOrder);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Order updated successfully. OrderId: {OrderId}, PaymentId: {PaymentId}, Status: {Status}",
                existingOrder.OrderId, existingOrder.PaymentId, existingOrder.Status);

            return existingOrder;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update the order. OrderId: {OrderId}", order.OrderId);
            throw new Exception("An error occurred while updating the order.", ex);
        }
    }

}

