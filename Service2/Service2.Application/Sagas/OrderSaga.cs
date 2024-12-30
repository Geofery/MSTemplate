using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;
using Application.Events;
using SharedMessages;
using Application.Commands;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Sagas
{
    public class OrderSaga : Saga<OrderSagaData>,
        IAmStartedByMessages<PlaceOrder>,
        IHandleMessages<UserValidated>,
        IHandleMessages<UserValidationFailed>,
        IHandleMessages<SignupCompleted>,
        IHandleMessages<SaveOrderCompleted>,
        IHandleMessages<PaymentProcessed>,
        IHandleMessages<PaymentFailed>
    {
        private readonly ILogger<OrderSaga> _logger;

        public OrderSaga(ILogger<OrderSaga> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handling PlaceOrder. OrderId: {OrderId}, Email: {Email}, Products: {Product}", message.OrderId, message.Email, message.Products);

            Data.OrderId = message.OrderId;
            Data.Name = message.Name;
            Data.Email = message.Email;
            Data.Password = message.Password;
            Data.Street = message.Street;
            Data.City = message.City;
            Data.PostalCode = message.PostalCode;
            Data.Products = message.Products;

            try
            {
                await context.Send(new ValidateUser { Email = Data.Email, OrderId = Data.OrderId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending ValidateUser command for OrderId: {OrderId}", message.OrderId);
                throw;
            }
        }

        public async Task Handle(UserValidated message, IMessageHandlerContext context)
        {
            _logger.LogInformation("UserValidated received. UserId: {UserId}, OrderId: {OrderId}", message.UserId, Data.OrderId);

            Data.UserId = message.UserId;

            try
            {
                _logger.LogInformation("THIS IS PRODUCTS: {product}", Data.Products);
                await context.Send(new SaveOrder
                {
                    UserId = Data.UserId,
                    OrderId = Data.OrderId,
                    Products = Data.Products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SaveOrder command for OrderId: {OrderId}", Data.OrderId);
                throw;
            }
        }

        public async Task Handle(UserValidationFailed message, IMessageHandlerContext context)
        {
            _logger.LogWarning("User validation failed. Reason: {Reason}, OrderId: {OrderId}", message.Reason, Data.OrderId);

            try
            {
                await context.Send(new SignupCommand
                {
                    Name = Data.Name,
                    Email = Data.Email,
                    Password = Data.Password,
                    Street = Data.Street,
                    City = Data.City,
                    PostalCode = Data.PostalCode,
                    OrderId = Data.OrderId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SignupCommand for OrderId: {OrderId}", Data.OrderId);
                throw;
            }
        }

        public async Task Handle(SignupCompleted message, IMessageHandlerContext context)
        {
            _logger.LogInformation("SignupCompleted received. UserId: {UserId}, OrderId: {OrderId}", message.UserId, Data.OrderId);

            Data.UserId = message.UserId;

            try
            {
                await context.Send(new SaveOrder
                {
                    UserId = Data.UserId,
                    OrderId = Data.OrderId,
                    Products = Data.Products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SaveOrder command for OrderId: {OrderId}", Data.OrderId);
                throw;
            }
        }

        public async Task Handle(SaveOrderCompleted message, IMessageHandlerContext context)
        {
            _logger.LogInformation("SaveOrderCompleted received. OrderId: {OrderId}", Data.OrderId);

            Data.Products = message.Products;

            try
            {
                await context.Send(new ProcessPayment
                {
                    OrderId = Data.OrderId,
                    Amount =  CalculateOrderAmount(Data.Products)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending ProcessPayment command for OrderId: {OrderId}", Data.OrderId);
                throw;
            }
        }

        public async Task Handle(PaymentProcessed message, IMessageHandlerContext context)
        {
            _logger.LogInformation("PaymentProcessed received. PaymentId: {PaymentId}, OrderId: {OrderId}", message.PaymentId, Data.OrderId);
            Data.PaymentId = message.PaymentId;
            await context.Send(new UpdateOrderPaymentStatus
            {
                OrderId = Data.OrderId,
                PaymentId = Data.PaymentId,
                Status = message.Status,
                Reason = message.Reason,
                Amount = message.Amount
            });

            MarkAsComplete();
        }

        public async Task Handle(PaymentFailed message, IMessageHandlerContext context)
        {
            _logger.LogWarning("PaymentFailed received. Reason: {Reason}, OrderId: {OrderId}", message.Reason, Data.OrderId);

            Data.PaymentId = message.PaymentId;
            try
            {
                await context.Send(new UpdateOrderPaymentStatus
                {
                    OrderId = Data.OrderId,
                    PaymentId = Data.PaymentId,
                    Status = message.Status,
                    Reason = message.Reason,
                    Amount = message.Amount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending CancelOrder command for OrderId: {OrderId}", Data.OrderId);
                throw;
            }

            MarkAsComplete();
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
        {
            mapper.MapSaga(saga => saga.OrderId)
                .ToMessage<PlaceOrder>(message => message.OrderId)
                .ToMessage<UserValidated>(message => message.OrderId)
                .ToMessage<UserValidationFailed>(message => message.OrderId)
                .ToMessage<SignupCompleted>(message => message.OrderId)
                .ToMessage<SaveOrderCompleted>(message => message.OrderId)
                .ToMessage<PaymentProcessed>(message => message.OrderId)
                .ToMessage<PaymentFailed>(message => message.OrderId);
        }

        private decimal CalculateOrderAmount(ICollection<Product> products)
        {
            decimal total = 0;
            Random r = new Random();

            foreach (var product in products)
            {
                int randomValue = r.Next(1, 101);
                total += product.Quantity * randomValue;
            }
            return total;
        }
    }
}
