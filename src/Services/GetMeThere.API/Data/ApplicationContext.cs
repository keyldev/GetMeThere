using GetMeThere.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GetMeThere.API.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {}
        public DbSet<User> Users { get; set; }
    }
}
