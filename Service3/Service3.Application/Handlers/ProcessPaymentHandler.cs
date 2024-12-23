using Application.Commands;
using Application.Events;
using NServiceBus;


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
                await context.Publish(new PaymentProcessed { OrderId = message.OrderId });
            }
            else
            {
                await context.Publish(new PaymentFailed
                {
                    OrderId = message.OrderId,
                    Reason = "Payment processing failed."
                });
            }
        }
    }
}
