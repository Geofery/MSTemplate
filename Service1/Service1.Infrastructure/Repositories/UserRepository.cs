using System;
using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<User> GetMemberByIdAsync(Guid id)
        {
            var result = await _dbContext.Users.FindAsync(id);
            if (result == null)
            {
                throw new Exception($"No User found with that id: {id}");
            }
            return result;
        }

        public async Task<IEnumerable<User>> GetAllMembersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> SaveMemberAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            var result = await _dbContext.Users.FindAsync(user.LeadId);
            if (result == null)
            {
                throw new Exception("User not saved or found");
            }
            return result;
        }

        public Task<bool> ValidateUserAsync(Guid userId)
        {
            //TODO Simulate user validation logic.
            return Task.FromResult(true);
        }

        public async Task<bool> HelthCheck()
        {
            return  await _dbContext.Database.CanConnectAsync();
        }
    }
}
