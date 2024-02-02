using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeatherReport.Models;

namespace WeatherReport.Data
{
    public class DataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<User>()
                .HasBaseType<IdentityUser<Guid>>();

            modelBuilder.Entity<UserCity>().HasKey(uc => new
            {
                uc.UserId,
                uc.CityId
            });

            modelBuilder.Entity<IdentityUserLogin<Guid>>().HasKey(x => x.UserId);
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasKey(x => x.UserId);
            modelBuilder.Entity<IdentityUserToken<Guid>>().HasKey(x => x.UserId);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<UserCity> UserCities { get; set; }
    }
}
