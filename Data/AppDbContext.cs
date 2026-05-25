using InventoryTool.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryTool.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<InventoryReport> InventoryReports => Set<InventoryReport>();
    public DbSet<InventoryReportLine> InventoryReportLines => Set<InventoryReportLine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .HasIndex(x => x.Code)
            .IsUnique();

        builder.Entity<Product>()
            .Property(x => x.Code)
            .HasMaxLength(50);

        builder.Entity<Product>()
            .Property(x => x.Name)
            .HasMaxLength(200);

        builder.Entity<InventoryReport>()
            .HasIndex(x => new { x.EmployeeId, x.ReportDate });

        builder.Entity<InventoryReportLine>()
            .Property(x => x.Quantity)
            .HasPrecision(18, 3);
    }
}