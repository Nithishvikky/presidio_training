using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Models;
using DSS.Models.DTOs;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IAuthSessionService _authSessionService;
    private readonly IRepository<Guid, AuthSession> _authSessionRepository;
    private readonly IBcryptionService _bcryptionService;

    public AuthService(
        IUserService userService,
        ITokenService tokenService,
        IAuthSessionService authSessionService,
        IRepository<Guid, AuthSession> authSessionRepository,
        IBcryptionService bcryptionService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _authSessionService = authSessionService;
        _authSessionRepository = authSessionRepository;
        _bcryptionService = bcryptionService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userService.GetUserByEmail(dto.Email);
        if (user == null || !_bcryptionService.VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return await GenerateSessionTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var session = await _authSessionService.GetByRefreshToken(refreshToken);

        if (session == null || session.IsRevoked || session.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        var user = await _userService.GetUserById(session.UserId);

        var tokens = new AuthResponse
        {
            AccessToken = await _tokenService.GenerateAccessToken(user),
            RefreshToken = await _tokenService.GenerateRefreshToken(),
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            }
        };

        session.RefreshToken = tokens.RefreshToken;
        session.CreatedAt = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.AddDays(7);

        await _authSessionRepository.Update(session.Id,session);

        return tokens;
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var session = await _authSessionService.GetByRefreshToken(refreshToken);
        if (session != null)
        {
            session.IsRevoked = true;
            await _authSessionRepository.Update(session.Id,session);
        }
    }

    private async Task<AuthResponse> GenerateSessionTokensAsync(User user)
    {
        var tokens = new AuthResponse
        {
            AccessToken = await _tokenService.GenerateAccessToken(user),
            RefreshToken = await _tokenService.GenerateRefreshToken(),
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role
            }
        };

        var session = new AuthSession
        {
            UserId = user.Id,
            RefreshToken = tokens.RefreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days for refresh token
            IsRevoked = false
        };

        await _authSessionRepository.Add(session);
        return tokens;
    }
}
