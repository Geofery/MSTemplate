using System.Collections.Generic;
using System.Reflection.Emit;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId); 
                entity.Property(o => o.UserId).IsRequired(); 

                // Define One-to-Many relationship with Product
                entity.HasMany(o => o.Products)
                      .WithOne(p => p.Order)
                      .HasForeignKey(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });

            
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId); 
                entity.Property(p => p.Quantity).IsRequired(); 

            
                entity.HasOne(p => p.Order)
                      .WithMany(o => o.Products)
                      .HasForeignKey(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}