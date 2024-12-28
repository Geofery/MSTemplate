using NServiceBus;
using SharedMessages;
using Microsoft.Extensions.Logging;
using Domain.Repositories;

namespace Application.Handlers
{
    public class ProcessPaymentHandler : IHandleMessages<ProcessPayment>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<ProcessPaymentHandler> _logger;

        public ProcessPaymentHandler(ILogger<ProcessPaymentHandler> logger, IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProcessPayment message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Processing payment for OrderId: {OrderId}, Amount: {Amount}", message.OrderId, message.Amount);

            try
            {
                var isPaymentSuccessful = await _paymentRepository.ProcessPaymentAsync(message.OrderId, message.Amount);

                if (isPaymentSuccessful)
                {
                    var paymentId = Guid.NewGuid();

                    _logger.LogInformation("Payment processed successfully. OrderId: {OrderId}, PaymentId: {PaymentId}", message.OrderId, paymentId);

                    await context.Publish(new PaymentProcessed
                    {
                        OrderId = message.OrderId,
                        PaymentId = paymentId,
                        Amount = message.Amount
                    });
                }
                else
                {
                    var paymentId = Guid.NewGuid();

                    _logger.LogWarning("Payment processing failed. OrderId: {OrderId}, Amount: {Amount}", message.OrderId, message.Amount);

                    await context.Publish(new PaymentFailed
                    {
                        OrderId = message.OrderId,
                        PaymentId = paymentId,
                        Amount = message.Amount,
                        Reason = "Payment processing failed, not enough funds."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment for OrderId: {OrderId}, Amount: {Amount}", message.OrderId, message.Amount);
                throw;
            }
        }
    }
}
