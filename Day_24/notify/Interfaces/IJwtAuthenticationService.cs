using Notify.Models.DTOs;

namespace Notify.Interfaces
{
    public interface IJwtAuthenticationService
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
    }
}