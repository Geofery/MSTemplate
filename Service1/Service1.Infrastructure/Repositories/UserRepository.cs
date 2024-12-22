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
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllMembersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> SaveMemberAsync(User user)
        {
            Console.WriteLine("IN REPO!!!!");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            var result = await _dbContext.Users.FindAsync(user.LeadId);
            return result;
        }

        public async Task<bool> HelthCheck()
        {
            return  await _dbContext.Database.CanConnectAsync();
        } 
    }
}
