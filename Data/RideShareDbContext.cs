using Microsoft.EntityFrameworkCore;
using ProjetInfo.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProjetInfo.Data
{
    // RideShareDbContext.cs
    public class RideShareDbContext : DbContext
    {
        public RideShareDbContext(DbContextOptions<RideShareDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<RideFeedback> RideFeedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.id);

            // Vehicle configuration
            modelBuilder.Entity<Vehicle>()
                .HasKey(v => v.VehicleID);

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Driver)
                .WithMany(d => d.Vehicles)
                .HasForeignKey(v => v.DriverID)
                .OnDelete(DeleteBehavior.NoAction); // Prevent multiple cascade paths

            // UserPreferences configuration
            modelBuilder.Entity<UserPreferences>()
                .HasKey(up => up.PreferenceID);

            modelBuilder.Entity<UserPreferences>()
                .HasOne(up => up.User)
                .WithOne(u => u.UserPreferences)
                .HasForeignKey<UserPreferences>(up => up.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Ride configuration
            modelBuilder.Entity<Ride>()
                .HasKey(r => r.RideID);

            modelBuilder.Entity<Ride>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rides)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ride>()
                .HasOne(r => r.Driver)
                .WithMany(d => d.Rides)
                .HasForeignKey(r => r.DriverID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Ride>()
                .HasOne(r => r.PaymentTransaction)
                .WithMany(pt => pt.Rides)
                .HasForeignKey(r => r.PaymentTransactionID)
                .OnDelete(DeleteBehavior.NoAction);

            // PaymentTransaction configuration
            modelBuilder.Entity<PaymentTransaction>()
                .HasKey(pt => pt.TransactionID);

            modelBuilder.Entity<PaymentTransaction>()
                .HasOne(pt => pt.User)
                .WithMany(u => u.PaymentTransactions)
                .HasForeignKey(pt => pt.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            // RideFeedback configuration
            modelBuilder.Entity<RideFeedback>()
                .HasKey(rf => rf.FeedbackID);

            modelBuilder.Entity<RideFeedback>()
                .HasOne(rf => rf.Ride)
                .WithMany(r => r.RideFeedbacks)
                .HasForeignKey(rf => rf.RideID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RideFeedback>()
                .HasOne(rf => rf.User)
                .WithMany(u => u.RideFeedbacks)
                .HasForeignKey(rf => rf.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RideFeedback>()
                .HasOne(rf => rf.Driver)
                .WithMany(d => d.DriverFeedbacks)
                .HasForeignKey(rf => rf.DriverID)
                .OnDelete(DeleteBehavior.NoAction);

            // Add indexes for performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.PlateNumber)
                .IsUnique();
        }
    }
}
