using System;
using System.Threading.Tasks;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SharedMessages;
/*
namespace Application.Handlers
{
    public class CancelOrderHandler : IHandleMessages<CancelOrder>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CancelOrderHandler> _logger;

        public CancelOrderHandler(IOrderRepository orderRepository, ILogger<CancelOrderHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CancelOrder message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handling CancelOrder command. OrderId: {OrderId}, PaymentId: {PaymentId}, Reason: {Reason}",
                message.OrderId, message.PaymentId, message.Reason);

            try
            {
                var updatedOrder = await _orderRepository.CancelOrderAsync(message);

                if (updatedOrder == null)
                {
                    _logger.LogWarning("Failed to cancel order. OrderId: {OrderId}", message.OrderId);
                    return;
                }

                _logger.LogInformation("Order canceled successfully. OrderId: {OrderId}", updatedOrder.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while canceling order. OrderId: {OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
*/