using ConsultingManagement.Models.DTOs;
using Microsoft.AspNetCore.Authentication;

namespace ConsultingManagement.Interfaces
{
    public interface IAuthenticationServiceCustom
    {
        public Task<UserLoginResponse> Login(UserLoginRequest user);
        public Task<UserLoginResponse> AuthenticateUser(AuthenticateResult result);
    }
}