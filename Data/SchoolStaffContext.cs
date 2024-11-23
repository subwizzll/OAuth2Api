using Microsoft.EntityFrameworkCore;
using SchoolStaffApi.Models;

namespace SchoolStaffApi.Data
{
    public class SchoolStaffContext : DbContext
    {
        public SchoolStaffContext(DbContextOptions<SchoolStaffContext> options) 
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.DateOfBirth)
                    .IsRequired();
            });
        }
    }
} 