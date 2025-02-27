using BikeMaintTracker.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeMaintTracker.Server
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Bikes> Bikes { get; set; }
        public DbSet<MaintLog> MaintLogs { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        public DbSet<AlertStatus> AlertStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Bikes>().ToTable("Bikes");
            // Configure entity mappings here
        }
    }
}
