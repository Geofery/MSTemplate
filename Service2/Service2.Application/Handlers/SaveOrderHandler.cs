using System;
using Application.Commands;
using SharedMessages;
using Domain.Repositories;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    //TODO: Add Logging
	public class SaveOrderHandler : IHandleMessages<SaveOrder>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<SaveOrderHandler> _logger;

        public SaveOrderHandler(IOrderRepository orderRepository, ILogger<SaveOrderHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger;
        }

        public async Task Handle(SaveOrder message, IMessageHandlerContext context)
        {
            var products = new List<Product>();

            foreach (var product in message.Products)
            {
                products.Add(new Product(product.Id, product.Quantity));
            }

            var order = new Order(message.OrderId, message.UserId, products);

            var response = await _orderRepository.SaveOrderAsync(order);
            if(response is null)
            {
                _logger.LogInformation("Save order failed: {order}", order);
            }

            _logger.LogInformation("Order saved succesfully: {order}", order);
            //TODO: Return order? 
        }
    }
}

