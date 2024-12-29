﻿using System;
using Domain.Models;
using SharedMessages;

namespace Domain.Repositories
{
	public interface IOrderRepository
	{
		Task<Order> SaveOrderAsync(Order order);
		Task<Order> CancelOrderAsync(CancelOrder cancelOrder);
        Task<Order> UpdateOrderAsync(Order order);
        Task<bool> HealthCheckAsync();
    }
}

