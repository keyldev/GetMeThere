using GetMeThere.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Data
{
    public class ApplicationContext : DbContext
    {
        #region DbSets
        public DbSet<User> Users { get; set; }


        #endregion
        public ApplicationContext()
        {
            
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {}
        
    }
}
