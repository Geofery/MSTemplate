using NServiceBus;
using Application.Events;
using SharedMessages;
using Application.Commands;
using Domain.Models;

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

    public async Task Handle(SaveOrderCompleted message, IMessageHandlerContext context)
    {
        Console.WriteLine("Order saved successfully. Initiating payment process.");
        Data.ProductsConverted = message.Products;
        // Send a command to PaymentService to process the payment
        //TODO: Needs updated Products list in Data. 
        await context.Send(new ProcessPayment
        {
            OrderId = Data.OrderId,
            Amount = CalculateOrderAmount(Data.ProductsConverted), // Implement this method to calculate the total amount
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
            .ToMessage<SaveOrderCompleted>(message => message.OrderId)
            .ToMessage<PaymentProcessed>(message => message.OrderId)
            .ToMessage<PaymentFailed>(message => message.OrderId);
    }

    private decimal CalculateOrderAmount(List<Product> products)
    {
        decimal sum = 0;
        Random r = new Random();

        foreach (var product in products)
        {
            sum += product.Quantity * r.NextInt64(100) + 1;
        }
        return sum;
    }
}