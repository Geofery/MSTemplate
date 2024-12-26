using Domain.Repositories;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SharedMessages;

namespace Application.Handlers
{
    public class CancelOrderHandler : IHandleMessages<CancelOrder>
    {

        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CancelOrderHandler> _logger;

        public CancelOrderHandler(IOrderRepository orderRepository, ILogger<CancelOrderHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task Handle(CancelOrder message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Cancelling order: {message.OrderId}, Reason: {message.Reason}");

            //TODO: update order with Reason and paymentfailed
            await Task.CompletedTask;
        }
    }
}

