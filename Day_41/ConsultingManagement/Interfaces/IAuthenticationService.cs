
using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
    }
}