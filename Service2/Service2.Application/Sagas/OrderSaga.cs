using NServiceBus;
using Application.Events;
using SharedMessages;
using Application.Commands;

namespace Application.Sagas;

public class OrderSaga : Saga<OrderSagaData>,
    IAmStartedByMessages<PlaceOrder>,
    IHandleMessages<UserValidated>,
    IHandleMessages<UserValidationFailed>,
    IHandleMessages<SignupCompleted>,
    IHandleMessages<PaymentProcessed>,
    IHandleMessages<PaymentFailed>
{
    //TODO: Add missing handlers
    public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
    {
        Console.WriteLine("In Saga PlaceOrder");
        Data.OrderId = message.OrderId;
        Data.Name = message.Name;
        Data.Email = message.Email;
        Data.Password = message.Password;
        Data.Street = message.Street;
        Data.City = message.City;
        Data.PostalCode = message.PostalCode;
        Data.Products = message.Products;

        // Request user validation
        await context.Send(new ValidateUser { Email = Data.Email });
    }

    public async Task Handle(UserValidated message, IMessageHandlerContext context)
    {
        Data.UserId = message.UserId;

        // Proceed to save the order
        await context.Send(new SaveOrder
        {
            UserId = Data.UserId,
            Products = Data.Products
        });
    }

    public async Task Handle(UserValidationFailed message, IMessageHandlerContext context)
    {
        //TODO: Add logger
        Console.WriteLine($"{message.Reason}: Creating new user.");
        // User doesn't exist, create a new user
        await context.Send(new SignupCommand
        {
            Name = Data.Name,
            OrderId = Data.OrderId,
            Email = Data.Email,
            Password = Data.Password,
            Street = Data.Street,
            City = Data.City,
            PostalCode = Data.PostalCode
        });
    }

    public async Task Handle(SignupCompleted message, IMessageHandlerContext context)
    {
        Data.UserId = message.UserId;
        await context.Send(new SaveOrder
        {
            UserId = Data.UserId,
            OrderId = Data.OrderId,
            Products = Data.Products,
        });
    }

    public async Task Handle(PaymentProcessed message, IMessageHandlerContext context)
    {
        Data.PaymentId = message.PaymentId;

        // Mark the saga as complete
        //TODO: Create OrderCompletedHandler
        MarkAsComplete();
        await context.Publish(new OrderCompleted
        {
            OrderId = Data.OrderId,
            PaymentId = Data.PaymentId
        });
    }

    public async Task Handle(PaymentFailed message, IMessageHandlerContext context)
    {
        Data.PaymentId = message.PaymentId;

        // Log the reason for failure or perform other
        //TODO: Create CancelOrderHandler
        await context.Send(new CancelOrder
        {
            OrderId = Data.OrderId,
            PaymentId = Data.PaymentId,
            Reason = message.Reason
        });

        // Mark the saga as complete
        MarkAsComplete();
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.OrderId)
              .ToMessage<PlaceOrder>(message => message.OrderId)
              .ToMessage<UserValidationFailed>(message => message.OrderId)
              .ToMessage<UserValidated>(message => message.OrderId)
              .ToMessage<PaymentProcessed>(message => message.OrderId)
              .ToMessage<PaymentFailed>(message => message.OrderId);
}
}