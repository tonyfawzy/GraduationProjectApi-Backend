using GraduationProjectApi.Models;
using Microsoft.EntityFrameworkCore;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ServiceTag> ServiceTags { get; set; }
    public DbSet<ServiceImage> ServiceImages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithOne(a => a.User)
            .HasForeignKey<Address>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceImage>()
            .HasOne(si => si.Service)
            .WithMany(s => s.ServiceImages)
            .HasForeignKey(si => si.ServiceId);

        modelBuilder.Entity<Service>()
            .HasOne(s => s.User)
            .WithMany(u => u.Services)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceTag>()
            .HasKey(st => new { st.ServiceId, st.TagId });

        modelBuilder.Entity<ServiceTag>()
            .HasOne(st => st.Service)
            .WithMany(s => s.ServiceTags)
            .HasForeignKey(st => st.ServiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceTag>()
            .HasOne(st => st.Tag)
            .WithMany(t => t.ServiceTags)
            .HasForeignKey(st => st.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    
    // public DbSet<Product> Products { get; set; }
    // public DbSet<Order> Orders { get; set; }
    
}