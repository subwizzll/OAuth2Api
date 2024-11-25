using Microsoft.EntityFrameworkCore;
using SchoolStaffAPI.Models;

namespace SchoolStaffAPI.Data;

public sealed class SchoolStaffContext(DbContextOptions<SchoolStaffContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity => entity.HasKey(e => e.Id));
    }
}