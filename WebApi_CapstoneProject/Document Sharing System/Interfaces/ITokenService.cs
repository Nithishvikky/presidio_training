using DSS.Models;

namespace DSS.Intrefaces
{
    public interface ITokenService
    {
        public Task<string> GenerateAccessToken(User user);
        public Task<string> GenerateRefreshToken();
    }
}