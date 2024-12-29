using Microsoft.EntityFrameworkCore;
using Domain;

namespace Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Payment> Payments { get; set; }
    }
}