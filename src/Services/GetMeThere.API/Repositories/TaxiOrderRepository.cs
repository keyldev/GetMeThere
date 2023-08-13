using GetMeThere.API.Data;

namespace GetMeThere.API.Repositories
{
    public class TaxiOrderRepository : ITaxiOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TaxiOrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }



    }
}
