using DSS.Contexts;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class AuthSessionRepository : Repository<Guid, AuthSession>
    {
        private readonly ILogger<AuthSessionRepository> _logger;
        public AuthSessionRepository(DssContext context, ILogger<AuthSessionRepository> logger) : base(context)
        {
            _logger = logger;
        }
        public override async Task<AuthSession> Get(Guid key)
        {
            try
            {
                var authSession = await _dssContext.AuthSessions.SingleOrDefaultAsync(u => u.Id == key);
                if (authSession== null)
                {
                    _logger.LogWarning("Session not found with ID: {SesssionId}", key);
                    throw new KeyNotFoundException("Session not found");
                }
                _logger.LogInformation("Fetched Session with ID: {SesssionId}", key);
                return authSession;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching authsession with ID: {SesssionId}", key);
                throw;
            }
        }

        public override async Task<IEnumerable<AuthSession>> GetAll()
        {
            try
            {
                var authSessions = await _dssContext.AuthSessions.ToListAsync();
                _logger.LogInformation("Fetched all sessions Count: {Count}", authSessions.Count);
                return authSessions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all sessions");
                throw;
            }
        }
    }
}