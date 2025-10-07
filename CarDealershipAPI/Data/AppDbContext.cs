using CarDealershipAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace CarDealershipAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :
        base(options)
        { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<OtpEntry> OtpEntries => Set<OtpEntry>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}
