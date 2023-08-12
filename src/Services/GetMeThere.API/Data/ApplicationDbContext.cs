using GetMeThere.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        #region DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TaxiOrder> TaxiOrders { get; set; }
        public DbSet<TaxiDriver> TaxiDrivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        #endregion
        public ApplicationDbContext()
        {
            
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        
    }
}
