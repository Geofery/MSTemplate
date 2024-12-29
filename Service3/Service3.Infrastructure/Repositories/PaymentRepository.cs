using System;
using System.Threading.Tasks;
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentRepository> _logger;

        public PaymentRepository(AppDbContext dbContext, ILogger<PaymentRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Payment> ProcessPaymentAsync(Payment payment)
        {
            if (payment.OrderId == Guid.Empty)
            {
                throw new ArgumentException("OrderId cannot be empty.", nameof(payment.OrderId));
            }

            if (payment.Amount <= 0)
            {
                throw new ArgumentException("Payment amount must be greater than zero.", nameof(payment.Amount));
            }

            try
            {
                
                // Simulate payment processing
                var isPaymentSuccessful = new Random().Next(2) == 0;
                payment.Status = isPaymentSuccessful ? "Processed" : "Failed";
                payment.Reason = isPaymentSuccessful ? "Sufficient funds" : "Insufficient funds";

                var result = await _dbContext.Payments.AddAsync(payment);
                await _dbContext.SaveChangesAsync();

                if(result.Entity is null)
                {
                    _logger.LogWarning("Payment with ID: {PaymentId} not found.", payment.Id);
                    throw new KeyNotFoundException($"Payment with ID: {payment.Id} not found.");
                }

                _logger.LogInformation("Payment {Status} for OrderId: {OrderId}, Amount: {Amount}",
                    payment.Status, payment.OrderId, payment.Amount);

                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment. OrderId: {OrderId}, Amount: {Amount}",
                    payment.OrderId, payment.Amount);
                throw new Exception("Payment processing failed.", ex);
            }
        }

        public async Task CancelPaymentAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("PaymentId cannot be empty.", nameof(paymentId));
            }

            try
            {
                var payment = await _dbContext.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);

                if (payment == null)
                {
                    _logger.LogWarning("Payment with ID: {PaymentId} not found for cancellation.", paymentId);
                    throw new KeyNotFoundException($"Payment with ID: {paymentId} not found.");
                }

                payment.Status = "Cancelled";

                _dbContext.Payments.Update(payment);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Payment cancelled successfully. PaymentId: {PaymentId}, Amount: {Amount}",
                    paymentId, payment.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while canceling payment. PaymentId: {PaymentId}", paymentId);
                throw new Exception("Payment cancellation failed.", ex);
            }
        }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                _logger.LogInformation("PaymentService Database connectivity check: {Status}", canConnect ? "Success" : "Failure");
                return canConnect;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connectivity health check failed.");
                return false;
            }
        }
    }
}
