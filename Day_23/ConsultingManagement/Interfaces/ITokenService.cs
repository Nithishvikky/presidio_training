using ConsultingManagement.Models;

namespace ConsultingManagement.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}