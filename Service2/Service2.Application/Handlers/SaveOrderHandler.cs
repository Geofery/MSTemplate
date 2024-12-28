using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Commands;
using SharedMessages;
using Domain.Repositories;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Application.Events;
using NServiceBus;

namespace Application.Handlers
{
    public class SaveOrderHandler : IHandleMessages<SaveOrder>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<SaveOrderHandler> _logger;

        public SaveOrderHandler(IOrderRepository orderRepository, ILogger<SaveOrderHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(SaveOrder message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handling SaveOrder command. OrderId: {OrderId}, UserId: {UserId}",
                message.OrderId, message.UserId);

            try
            {
                var products = new List<Product>();
                foreach (var product in message.Products)
                {
                    products.Add(new Product(product.Id, product.Quantity));
                }

                var order = new Order(message.OrderId, message.UserId, products);
                var savedOrder = await _orderRepository.SaveOrderAsync(order);

                if (savedOrder == null)
                {
                    _logger.LogWarning("Failed to save order. OrderId: {OrderId}", message.OrderId);
                    return;
                }

                _logger.LogInformation("Order saved successfully. OrderId: {OrderId}", savedOrder.OrderId);

                await context.Publish(new SaveOrderCompleted
                {
                    OrderId = savedOrder.OrderId,
                    Products = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving order. OrderId: {OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
