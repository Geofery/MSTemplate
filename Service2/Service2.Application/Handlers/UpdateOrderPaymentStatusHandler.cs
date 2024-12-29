using System;
using System.Threading.Tasks;
using Application.Commands;
using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace Application.Handlers
{
    public class UpdateOrderPaymentStatusHandler : IHandleMessages<UpdateOrderPaymentStatus>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<UpdateOrderPaymentStatusHandler> _logger;

        public UpdateOrderPaymentStatusHandler(IOrderRepository orderRepository, ILogger<UpdateOrderPaymentStatusHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(UpdateOrderPaymentStatus message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handling UpdateOrderPaymentStatus command. OrderId: {OrderId}, PaymentId: {PaymentId}, Status: {Status}",
                message.OrderId, message.PaymentId, message.Status);

            try
            {
                var order = new Order(message.OrderId, message.PaymentId, message.Status, message.Reason, message.Amount);
                var result = await _orderRepository.UpdateOrderAsync(order);

                if (result is null)
                {
                    _logger.LogWarning("Failed to update order payment status. OrderId: {OrderId}", message.OrderId);
                    return;
                }

                _logger.LogInformation("Order payment status updated successfully. OrderId: {OrderId}, PaymentId: {PaymentId}, Status: {Status}, Reason: {Reason}",
                    message.OrderId, message.PaymentId, message.Status, message.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating order payment status. OrderId: {OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
