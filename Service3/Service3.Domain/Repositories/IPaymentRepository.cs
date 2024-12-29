using System;
namespace Domain.Repositories
{
	public interface IPaymentRepository
	{
            Task<bool> ProcessPaymentAsync(Guid orderId, decimal amount);
            Task CancelPaymentAsync(Guid paymentId);
            Task<bool> HealthCheckAsync();

    }
}


