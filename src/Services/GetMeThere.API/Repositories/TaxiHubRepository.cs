using GetMeThere.API.Data;

namespace GetMeThere.API.Repositories
{
    public class TaxiHubRepository : ITaxiHubRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public TaxiHubRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> UpdateUserConnectionId(string username, string connectionId)
        {
            using (var dbContext = _dbContext)
            {
                var user = dbContext.Users.FirstOrDefault(u=> u.Login == username);
                if(user == null)
                {
                    return false;
                }
                user.ConnectionId = connectionId;
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
