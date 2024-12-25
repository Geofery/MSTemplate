using System.Collections.Generic;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId); 
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(255);

                entity.HasOne(u => u.Address)
                      .WithOne(a => a.User)
                      .HasForeignKey<Address>(a => a.UserId); 
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(a => a.Id); 
                entity.Property(a => a.Street).HasMaxLength(100);
                entity.Property(a => a.City).HasMaxLength(50);
                entity.Property(a => a.PostalCode).HasMaxLength(10);
            });
        }
    }
}
