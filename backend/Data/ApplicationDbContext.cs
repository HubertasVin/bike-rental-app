using Microsoft.EntityFrameworkCore;
using BikeRentalApp.Domain.Entities;

namespace BikeRentalApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Bike> Bikes { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Rental> Rentals { get; set; } = null!;
    public DbSet<Zone> Zones { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<Authentication> Authentications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bike>()
            .HasOne(b => b.Zone)
            .WithMany(z => z.Bikes)
            .HasForeignKey(b => b.ZoneId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Bike)
            .WithMany(b => b.Rentals)
            .HasForeignKey(r => r.BikeId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.User)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.StartZone)
            .WithMany()
            .HasForeignKey(r => r.StartZoneId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.EndZone)
            .WithMany()
            .HasForeignKey(r => r.EndZoneId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Rental)
            .WithOne(r => r.Payment)
            .HasForeignKey<Payment>(p => p.RentalId);
    }
}
