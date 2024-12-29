using System;
namespace Domain.Repositories
{
	public interface IPaymentRepository
	{
            Task<Payment> ProcessPaymentAsync(Payment payment);
            Task CancelPaymentAsync(Guid paymentId);
            Task<bool> HealthCheckAsync();

    }
}


