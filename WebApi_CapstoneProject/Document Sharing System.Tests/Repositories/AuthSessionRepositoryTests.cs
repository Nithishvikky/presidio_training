using DSS.Contexts;
using DSS.Models;
using DSS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;


namespace Document_Sharing_System.Tests.Repositories
{
    [TestFixture]
    public class AuthSessionRepositoryTests
    {
        private DssContext _context;
        private AuthSessionRepository _authSessionRepository;
        private Mock<ILogger<AuthSessionRepository>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DssContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DssContext(options);
            _loggerMock = new Mock<ILogger<AuthSessionRepository>>();
            _authSessionRepository = new AuthSessionRepository(_context, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_WhenSessionExists_ReturnsSession()
        {
            // Arrange
            var id = Guid.NewGuid();
            var session = new AuthSession
            {
                Id = id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                UserId = Guid.NewGuid()
            };

            _context.AuthSessions.Add(session);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authSessionRepository.Get(id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public void Get_WhenSessionNotExists_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _authSessionRepository.Get(id);
            });

            Assert.That(ex.Message, Is.EqualTo("Session not found"));
        }

        [Test]
        public async Task GetAll_WhenSessionsExist_ReturnsAll()
        {
            // Arrange
            var sessions = new List<AuthSession>
            {
                new AuthSession { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddMinutes(30) },
                new AuthSession { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddMinutes(45) }
            };

            _context.AuthSessions.AddRange(sessions);
            await _context.SaveChangesAsync();

            // Act
            var result = await _authSessionRepository.GetAll();

            // Assert
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetAll_WhenNoSessionsExist_ReturnsEmpty()
        {
            // Act
            var result = await _authSessionRepository.GetAll();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
