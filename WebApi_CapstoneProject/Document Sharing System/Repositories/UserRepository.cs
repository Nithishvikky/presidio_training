using DSS.Contexts;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class UserRepository : Repository<Guid, User>
    {
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(DssContext context, ILogger<UserRepository> logger) : base(context)
        {
            _logger = logger;

        }
        public override async Task<User> Get(Guid key)
        {
            try
            {
                var user = await _dssContext.Users.SingleOrDefaultAsync(u => u.Id == key);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", key);
                    throw new KeyNotFoundException("No user found");
                }

                _logger.LogInformation("Fetched user with ID: {UserId}", key);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user with ID: {UserId}", key);
                throw;
            }

        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                var users = await _dssContext.Users.ToListAsync();
                _logger.LogInformation("Fetched all users. Count: {Count}", users.Count);
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all users");
                throw;
            }
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var TotalUsers = (await _dssContext.Users.ToListAsync()).Count;
            var TotalDocs = (await _dssContext.UserDocuments.ToListAsync()).Count;
            var TotalShared = (await _dssContext.DocumentShares.Where(d => !d.IsRevoked).ToListAsync()).Count;
            var TotalViews = (await _dssContext.DocumentViews.ToListAsync()).Count;

            var dto = new DashboardDto
            {
                TotalUsers = TotalUsers,
                TotalDocuments = TotalDocs,
                TotalShared = TotalShared,
                TotalViews = TotalViews
            };
            return dto;
        }
    }
}