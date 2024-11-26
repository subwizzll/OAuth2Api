using Microsoft.EntityFrameworkCore;
using SchoolStaffAPI.Models;
using SchoolStaffAPI.Models.Entity;

namespace SchoolStaffAPI.Data;

public sealed class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity => entity.HasKey(e => e.Id));
        
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}