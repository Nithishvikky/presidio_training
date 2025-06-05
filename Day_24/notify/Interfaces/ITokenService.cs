using Notify.Models;

namespace Notify.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}