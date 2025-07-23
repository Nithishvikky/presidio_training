using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Models;
using DSS.Models.DTOs;
using DSS.Services;
using Microsoft.AspNetCore.Http;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IAuthSessionService _authSessionService;
    private readonly IRepository<Guid, AuthSession> _authSessionRepository;
    private readonly IBcryptionService _bcryptionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserActivityLogService _userActivityLogService;

    public AuthService(
        IUserService userService,
        ITokenService tokenService,
        IAuthSessionService authSessionService,
        IRepository<Guid, AuthSession> authSessionRepository,
        IBcryptionService bcryptionService,
        IHttpContextAccessor httpContextAccessor,
        IUserActivityLogService userActivityLogService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _authSessionService = authSessionService;
        _authSessionRepository = authSessionRepository;
        _bcryptionService = bcryptionService;
        _httpContextAccessor = httpContextAccessor;
        _userActivityLogService = userActivityLogService;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userService.GetUserByEmail(dto.Email);
        if (user == null || !_bcryptionService.VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Update last login time
        user.LastLogin = DateTime.UtcNow;
        await _userService.UpdateUserLastLogin(user.Id, user.LastLogin.Value);

        // Log user activity for login
        try
        {
            var activityDto = new CreateActivityLogDto
            {
                UserId = user.Id,
                ActivityType = "Login",
                Description = $"User logged in successfully from {GetClientIPAddress()}"
            };
            await _userActivityLogService.LogActivityAsync(activityDto);
        }
        catch (Exception ex)
        {
            // Log warning but don't fail the login process
            Console.WriteLine($"Failed to log login activity for user {user.Id}: {ex.Message}");
        }

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

            // Log user activity for logout
            try
            {
                var activityDto = new CreateActivityLogDto
                {
                    UserId = session.UserId,
                    ActivityType = "Logout",
                    Description = $"User logged out successfully from {GetClientIPAddress()}"
                };
                await _userActivityLogService.LogActivityAsync(activityDto);
            }
            catch (Exception ex)
            {
                // Log warning but don't fail the logout process
                Console.WriteLine($"Failed to log logout activity for user {session.UserId}: {ex.Message}");
            }
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

    private string GetClientIPAddress()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Try to get the real IP address from various headers
                var forwardedHeader = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedHeader))
                {
                    return forwardedHeader.Split(',')[0].Trim();
                }

                var realIPHeader = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIPHeader))
                {
                    return realIPHeader;
                }

                return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
        }
        catch
        {
            // If we can't get the IP address, return a default value
        }
        return "Unknown";
    }
}
