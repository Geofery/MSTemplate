﻿using Domain.Models;

namespace Domain.Repositories
{
	public interface IUserRepository
	{
		Task<User> SaveMemberAsync(User user);
        Task<User> GetMemberByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllMembersAsync();
        Task<Guid> ValidateUserAsync(String Email);
        Task<bool> HealthCheckAsync();
    }
}

