using NServiceBus;
using SharedMessages;
using Microsoft.Extensions.Logging;
using Domain.Repositories;
using Domain;

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
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = message.OrderId,
                    Amount = message.Amount,
                };

                payment = await _paymentRepository.ProcessPaymentAsync(payment);
                await PublishMessage(context, payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment for OrderId: {OrderId}, Amount: {Amount}", message.OrderId, message.Amount);
                throw;
            }

            async Task PublishMessage(IMessageHandlerContext context, Payment payment)
            {
                if (payment.Status == "Processed")
                {
                    _logger.LogInformation("Payment processed successfully. OrderId: {OrderId}, PaymentId: {PaymentId}", payment.OrderId, payment.Id);
                    await context.Publish(new PaymentProcessed
                    {
                        OrderId = message.OrderId,
                        PaymentId = payment.Id,
                        Amount = payment.Amount,
                        Reason = payment.Reason,
                        Status = payment.Status
                    });
                }

                else
                {
                    _logger.LogWarning("Payment processing failed. OrderId: {OrderId}, Amount: {Amount}", payment.OrderId, message.Amount);

                    await context.Publish(new PaymentFailed
                    {
                        OrderId = payment.OrderId,
                        PaymentId = payment.Id,
                        Amount = payment.Amount,
                        Status = payment.Status,
                        Reason = payment.Reason
                    });
                }
            }
        }
    }
}
