using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IAuthSessionService
    {
        public Task<AuthSession> GetByRefreshToken(string RefreshToken);
        public Task<AuthSession> GetByUserId(Guid userId);
    }
}