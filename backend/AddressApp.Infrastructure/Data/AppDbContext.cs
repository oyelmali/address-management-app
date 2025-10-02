using Microsoft.EntityFrameworkCore;
using AddressApp.Core.Entities;

namespace AddressApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Address> Addresses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Region).IsRequired().HasMaxLength(100);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BranchNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BranchType).HasMaxLength(100);
            entity.Property(e => e.Street).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.WorkingHoursWeekdays).HasMaxLength(100);
            entity.Property(e => e.WorkingHoursWeekend).HasMaxLength(100);
            entity.Property(e => e.OriginalText).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}