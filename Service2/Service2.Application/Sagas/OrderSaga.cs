using NServiceBus;
using Application.Events;
using SharedMessages;
using Application.Commands;

namespace Application.Sagas;

public class OrderSaga : Saga<OrderSagaData>,
    IAmStartedByMessages<PlaceOrder>,
    IHandleMessages<UserValidated>,
    IHandleMessages<UserValidationFailed>,
    IHandleMessages<PaymentProcessed>,
    IHandleMessages<PaymentFailed>
{
    /*private readonly ILogger<OrderSaga> _logger;

    public OrderSaga(Ilogger<OrderSaga> logger)
    {
        _logger = logger;
    }*/

    public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
    {
        UpdateSagaData(message);
        await context.SendLocal().ConfigureAwait(false);
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
        mapper.MapSaga(saga => saga.OrderId)
              .ToMessage<OrderPlaced>(message => message.OrderId);
    }

    private void UpdateSagaData(PlaceOrder placeOrder)
    {
        Data.Name = placeOrder.Name;
        Data.Email = placeOrder.Email;
        Data.Password = placeOrder.Password;
        Data.Street = placeOrder.Street;
        Data.City = placeOrder.City;
        Data.PostalCode = placeOrder.PostalCode;
        Data.Products = placeOrder.Products;
    }
}
