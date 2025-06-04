using ConsultingManagement.Models.DTOs;

namespace ConsultingManagement.Interfaces
{
    public interface IAuthenticationServiceCustom
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
        public Task<UserLoginResponse> GoogleLogin(string email);
    }
}