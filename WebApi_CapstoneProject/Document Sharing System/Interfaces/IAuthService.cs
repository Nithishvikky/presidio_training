using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponse> LoginAsync(LoginRequestDto dto);
        public Task<AuthResponse> RefreshAsync(string refreshToken);
        public Task LogoutAsync(string refreshToken);
    }
}
