using DSS.Contexts;
using DSS.Models;
using DSS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DSS.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private DssContext _context;
        private UserRepository _userRepository;
        private Mock<ILogger<UserRepository>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DssContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DssContext(options);
            _loggerMock = new Mock<ILogger<UserRepository>>();
            _userRepository = new UserRepository(_context, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "Nithish" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.Get(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userId));
            Assert.That(result.Username, Is.EqualTo("Nithish"));
        }

        [Test]
        public void Get_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _userRepository.Get(nonExistentId);
            });

            Assert.That(ex.Message, Is.EqualTo("No user found"));
        }

        [Test]
        public async Task GetAll_WhenUsersExist_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Username = "User1" },
                new User { Id = Guid.NewGuid(), Username = "User2" }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetAll();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetAll_WhenNoUsersExist_ReturnsEmptyList()
        {
            // Act
            var result = await _userRepository.GetAll();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
