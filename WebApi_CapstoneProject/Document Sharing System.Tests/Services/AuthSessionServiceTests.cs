using DSS.Interfaces;
using DSS.Models;
using DSS.Services;
using Moq;


[TestFixture]
public class AuthSessionServiceTests
{
    private Mock<IRepository<Guid, AuthSession>> _authSessionRepoMock;
    private AuthSessionService _authSessionService;

    [SetUp]
    public void Setup()
    {
        _authSessionRepoMock = new Mock<IRepository<Guid, AuthSession>>();
        _authSessionService = new AuthSessionService(_authSessionRepoMock.Object);
    }

    [Test]
    public async Task GetByRefreshToken_ValidToken_ReturnsSession()
    {
        // Arrange
        var session = new AuthSession { RefreshToken = "validToken", UserId = Guid.NewGuid() };
        _authSessionRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<AuthSession> { session });

        // Act
        var result = await _authSessionService.GetByRefreshToken("validToken");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.RefreshToken, Is.EqualTo("validToken"));
    }

    [Test]
    public void GetByRefreshToken_InvalidToken_ThrowsKeyNotFoundException()
    {
        _authSessionRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<AuthSession>());

        Assert.That(async () =>
            await _authSessionService.GetByRefreshToken("invalidToken"),
            Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo("session not found"));
    }

    [Test]
    public async Task GetByUserId_ValidUserId_ReturnsSession()
    {
        var userId = Guid.NewGuid();
        var session = new AuthSession { UserId = userId, RefreshToken = "refresh123" };
        _authSessionRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<AuthSession> { session });

        var result = await _authSessionService.GetByUserId(userId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(userId));
    }

    [Test]
    public void GetByUserId_InvalidUserId_ThrowsKeyNotFoundException()
    {
        _authSessionRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<AuthSession>());

        Assert.That(async () =>
            await _authSessionService.GetByUserId(Guid.NewGuid()),
            Throws.TypeOf<KeyNotFoundException>().With.Message.EqualTo("session not found"));
    }
}
