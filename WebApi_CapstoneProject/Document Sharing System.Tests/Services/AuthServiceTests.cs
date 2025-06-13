using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Models;
using DSS.Models.DTOs;
using Moq;


[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserService> _userServiceMock;
    private Mock<ITokenService> _tokenServiceMock;
    private Mock<IAuthSessionService> _authSessionServiceMock;
    private Mock<IRepository<Guid, AuthSession>> _authSessionRepoMock;
    private Mock<IBcryptionService> _bcryptMock;
    private AuthService _authService;

    [SetUp]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _authSessionServiceMock = new Mock<IAuthSessionService>();
        _authSessionRepoMock = new Mock<IRepository<Guid, AuthSession>>();
        _bcryptMock = new Mock<IBcryptionService>();

        _authService = new AuthService(
            _userServiceMock.Object,
            _tokenServiceMock.Object,
            _authSessionServiceMock.Object,
            _authSessionRepoMock.Object,
            _bcryptMock.Object
        );
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", PasswordHash = "hashed" };
        var dto = new LoginRequestDto { Email = "test@example.com", Password = "plain" };
        var expected = new AuthResponse { AccessToken = "access", RefreshToken = "refresh" };

        _userServiceMock.Setup(x => x.GetUserByEmail(dto.Email)).ReturnsAsync(user);
        _bcryptMock.Setup(x => x.VerifyPassword(dto.Password, user.PasswordHash)).Returns(true);
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(user)).ReturnsAsync(expected.AccessToken);
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).ReturnsAsync(expected.RefreshToken);
        _authSessionRepoMock.Setup(x => x.Add(It.IsAny<AuthSession>())).ReturnsAsync(new AuthSession());

        var result = await _authService.LoginAsync(dto);

        Assert.That(result.AccessToken, Is.EqualTo("access"));
        Assert.That(result.RefreshToken, Is.EqualTo("refresh"));
    }

    [Test]
    public void LoginAsync_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User { Email = "user@test.com", PasswordHash = "hashed" };
        var dto = new LoginRequestDto { Email = "user@test.com", Password = "wrong" };

        _userServiceMock.Setup(x => x.GetUserByEmail(dto.Email)).ReturnsAsync(user);
        _bcryptMock.Setup(x => x.VerifyPassword(dto.Password, user.PasswordHash)).Returns(false);

        Assert.That(() => _authService.LoginAsync(dto), Throws.TypeOf<UnauthorizedAccessException>());
    }

    [Test]
    public async Task RefreshAsync_ValidToken_ReturnsNewAuthResponse()
    {
        var session = new AuthSession
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        var user = new User { Id = session.UserId };
        var expected = new AuthResponse { AccessToken = "new-access", RefreshToken = "new-refresh" };

        _authSessionServiceMock.Setup(x => x.GetByRefreshToken("refresh")).ReturnsAsync(session);
        _userServiceMock.Setup(x => x.GetUserById(session.UserId)).ReturnsAsync(user);
        _tokenServiceMock.Setup(x => x.GenerateAccessToken(user)).ReturnsAsync(expected.AccessToken);
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).ReturnsAsync(expected.RefreshToken);
        _authSessionRepoMock.Setup(x => x.Update(session.Id, It.IsAny<AuthSession>())).ReturnsAsync(session);

        var result = await _authService.RefreshAsync("refresh");

        Assert.That(result.AccessToken, Is.EqualTo("new-access"));
        Assert.That(result.RefreshToken, Is.EqualTo("new-refresh"));
    }

    [Test]
    public void RefreshAsync_ExpiredToken_ThrowsUnauthorizedAccessException()
    {
        var session = new AuthSession
        {
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
        };

        _authSessionServiceMock.Setup(x => x.GetByRefreshToken("expired")).ReturnsAsync(session);

        Assert.That(() => _authService.RefreshAsync("expired"), Throws.TypeOf<UnauthorizedAccessException>());
    }

    [Test]
    public async Task LogoutAsync_RevokesSession()
    {
        var session = new AuthSession { Id = Guid.NewGuid() };

        _authSessionServiceMock.Setup(x => x.GetByRefreshToken("refresh")).ReturnsAsync(session);
        _authSessionRepoMock.Setup(x => x.Update(session.Id, It.IsAny<AuthSession>())).ReturnsAsync(session);

        await _authService.LogoutAsync("refresh");

        Assert.That(session.IsRevoked, Is.True);
    }
}
