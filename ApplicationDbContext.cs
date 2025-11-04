using GraduationProjectApi.Models;
using Microsoft.EntityFrameworkCore;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<ServiceImage> ServiceImages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ServiceImage>()
            .HasOne(si => si.Service)
            .WithMany(s => s.ServiceImages)
            .HasForeignKey(si => si.ServiceId);

        modelBuilder.Entity<Service>()
            .HasOne(t => t.Trade)
            .WithMany()
            .HasForeignKey(t => t.TradeId);

        modelBuilder.Entity<Service>()
            .HasOne(l => l.Location)
            .WithMany()
            .HasForeignKey(l => l.LocationId);
        
        modelBuilder.Entity<Service>()
            .HasOne(u => u.User)
            .WithMany(us => us.Services)
            .HasForeignKey(u => u.UserId);
    }

    
    // public DbSet<Product> Products { get; set; }
    // public DbSet<Order> Orders { get; set; }
    
}