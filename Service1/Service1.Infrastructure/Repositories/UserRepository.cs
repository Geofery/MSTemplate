using System;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> GetMemberByIdAsync(Guid id)
        {
            try
            {
                var user = await _dbContext.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("No user found with ID: {UserId}", id);
                    throw new KeyNotFoundException($"No user found with ID: {id}");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user by ID: {UserId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllMembersAsync()
        {
            try
            {
                return await _dbContext.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                throw new Exception("Failed to retrieve users.", ex);
            }
        }

        public async Task<User> SaveMemberAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            try
            {
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("User saved successfully. UserId: {UserId}", user.UserId);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the user. UserId: {UserId}", user.UserId);
                throw new Exception("Failed to save the user.", ex);
            }
        }

        public async Task<Guid> ValidateUserAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));
            }

            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    _logger.LogWarning("No user found with email: {Email}", email);
                    return Guid.Empty;
                }

                _logger.LogInformation("User validated successfully. UserId: {UserId}, Email: {Email}", user.UserId, email);
                return user.UserId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating user with email: {Email}", email);
                throw new Exception("Failed to validate user.", ex);
            }
        }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var canConnect = await _dbContext.Database.CanConnectAsync();
                _logger.LogInformation("Database connectivity check: {Status}", canConnect ? "Success" : "Failure");
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
