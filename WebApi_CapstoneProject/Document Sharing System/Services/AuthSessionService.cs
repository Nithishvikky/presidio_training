using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Misc;
using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Services
{
    public class AuthSessionService : IAuthSessionService
    {
        private readonly IRepository<Guid, AuthSession> _authSessionRepository;

        public AuthSessionService(IRepository<Guid, AuthSession> authSessionRepository)
        {
            _authSessionRepository = authSessionRepository;
        }

        public async Task<AuthSession> GetByRefreshToken(string RToken)
        {
            var sessions = await _authSessionRepository.GetAll();
            var session = sessions.SingleOrDefault(s => s.RefreshToken.Equals(RToken));

            if (session == null)
            {
                throw new KeyNotFoundException("session not found");
            }
            return session;
        }

        public async Task<AuthSession> GetByUserId(Guid userId)
        {
            var sessions = await _authSessionRepository.GetAll();
            var session = sessions.SingleOrDefault(s => s.UserId == userId);

            if (session == null)
            {
                throw new KeyNotFoundException("session not found");
            }
            return session;
        }
    }
}
