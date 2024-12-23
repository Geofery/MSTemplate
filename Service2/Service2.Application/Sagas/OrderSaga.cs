using NServiceBus;
using Application.Commands;
using Application.Events;
using SharedMessages;

namespace Application.Sagas;

public class OrderSaga : Saga<OrderSagaData>,
    IAmStartedByMessages<OrderPlaced>,
    IHandleMessages<UserValidated>,
    IHandleMessages<UserValidationFailed>,
    IHandleMessages<PaymentProcessed>,
    IHandleMessages<PaymentFailed>
{
    public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        Data.OrderId = message.OrderId;
        Data.User = message.User;

        await context.Send(new ValidateUser
        {
            UserId = Data.User.Id,
            Address = Data.User.Address
        });
    }

    public async Task Handle(UserValidated message, IMessageHandlerContext context)
    {
        await context.Send(new ProcessPayment
        {
            OrderId = Data.OrderId,
            Amount = message.TotalAmount
        });
    }

    public Task Handle(UserValidationFailed message, IMessageHandlerContext context)
    {
        MarkAsComplete();
        // Optionally publish or log failure
        return Task.CompletedTask;
    }

    public Task Handle(PaymentProcessed message, IMessageHandlerContext context)
    {
        MarkAsComplete();
        return context.Publish(new OrderCompleted
        {
            OrderId = Data.OrderId
        });
    }

    public Task Handle(PaymentFailed message, IMessageHandlerContext context)
    {
        MarkAsComplete();
        // Optionally flag the order for manual review
        return Task.CompletedTask;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
    {
        throw new NotImplementedException();
    }
}
