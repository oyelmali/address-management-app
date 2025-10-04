using Microsoft.EntityFrameworkCore;
using AddressApp.Domain.Entities;

namespace AddressApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                // Indexes for better search performance
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.Region);
                entity.HasIndex(e => e.BranchNumber);
                
                // Properties
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                
                entity.Property(e => e.Region)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.BranchNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.BranchType)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(200);
                
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.WorkingHoursWeekdays)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.WorkingHoursWeekend)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}