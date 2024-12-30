using NServiceBus;
using SharedMessages;
using Microsoft.Extensions.Logging;
using Domain.Repositories;

namespace Application.Handlers
{
    public class PaymentFailedHandler : IHandleMessages<PaymentFailed>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentFailedHandler> _logger;

        public PaymentFailedHandler(ILogger<PaymentFailedHandler> logger, IPaymentRepository paymentRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        }

        public async Task Handle(PaymentFailed message, IMessageHandlerContext context)
        {
            _logger.LogInformation("IN HANDLER");
            _logger.LogWarning("Payment failed for OrderId: {OrderId}, PaymentId: {PaymentId}, Reason: {Reason}",
                message.OrderId, message.PaymentId, message.Reason);

            try
            {
                await _paymentRepository.CancelPaymentAsync(message.PaymentId);

                _logger.LogInformation("Payment cancellation recorded for PaymentId: {PaymentId}, Reason: {Reason}",
                    message.PaymentId, message.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling failed payment. PaymentId: {PaymentId}, Reason: {Reason}",
                    message.PaymentId, message.Reason);
                throw;
            }
        }
    }
}
