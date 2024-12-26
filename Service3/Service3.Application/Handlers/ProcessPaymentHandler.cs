using NServiceBus;
using SharedMessages;

namespace Application.Handlers
{
    public class ProcessPaymentHandler : IHandleMessages<ProcessPayment>
    {
        public async Task Handle(ProcessPayment message, IMessageHandlerContext context)
        {
            // Simulate payment processing
            bool isPaymentSuccessful = true;

            if (isPaymentSuccessful)
            {
                await context.Publish(new PaymentProcessed
                {
                    OrderId = message.OrderId,
                    PaymentId = new Guid(),
                    Amount = message.Amount
                }) ;
            }
            else
            {
                await context.Publish(new PaymentFailed
                {
                    OrderId = message.OrderId,
                    PaymentId = new Guid(),
                    Amount = message.Amount,
                    Reason = "Payment processing failed."
                }) ;
            }
        }
    }
}
