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
            var result = await _dbContext.Users.FindAsync(user.UserId);
            if (result == null)
            {
                throw new Exception("User not saved or found");
            }
            //TODO: DET SOM SENDES RETUR SKAL INDEHOLDE ORDERID
            return result;
        }

        public async Task<Guid> ValidateUserAsync(string email)
        {
            var result = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (result == null)
            {
                return Guid.Empty;
            }
            return result.UserId;
        }

        public async Task<bool> HelthCheck()
        {
            return  await _dbContext.Database.CanConnectAsync();
        }
    }
}
