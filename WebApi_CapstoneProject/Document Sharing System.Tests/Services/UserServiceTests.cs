using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Models;
using DSS.Models.DTOs;
using DSS.Services;
using Microsoft.Extensions.Logging;
using Moq;


namespace DSS.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IRepository<Guid, User>> _userRepoMock;
        private Mock<IBcryptionService> _bcryptMock;
        private Mock<ILogger<UserService>> _loggerMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IRepository<Guid, User>>();
            _bcryptMock = new Mock<IBcryptionService>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _userService = new UserService(_userRepoMock.Object, _bcryptMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task AddUser_ShouldAddUser_WhenEmailIsUnique()
        {
            var userDto = new UserAddRequestDto
            {
                Username = "John",
                Email = "john@example.com",
                Password = "password",
                Role = "Admin"
            };

            var mappedUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "John",
                Email = "john@example.com",
                Role = "Admin"
            };

            _userRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());
            _bcryptMock.Setup(b => b.HashPassword("password")).Returns("hashed_pw");
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(mappedUser);

            var result = await _userService.AddUser(userDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("john@example.com"));
        }

        [Test]
        public void AddUser_ShouldThrow_WhenEmailExists()
        {
            var userDto = new UserAddRequestDto
            {
                Email = "existing@example.com",
                Password = "pass"
            };

            var existingUser = new User { Email = "existing@example.com" };
            _userRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { existingUser });

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _userService.AddUser(userDto));
        }

        [Test]
        public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
        {
            var users = new List<User>
            {
                new User { Email = "john@example.com" }
            };

            _userRepoMock.Setup(r => r.GetAll()).ReturnsAsync(users);

            var result = await _userService.GetUserByEmail("john@example.com");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("john@example.com"));
        }

        [Test]
        public void GetUserByEmail_ShouldThrow_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());

            Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.GetUserByEmail("notfound@example.com"));
        }

        [Test]
        public async Task GetUserById_ShouldReturnUser()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);

            var result = await _userService.GetUserById(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task UpdateUserPassword_ShouldUpdatePassword_WhenOldPasswordValid()
        {
            var userId = Guid.NewGuid();
            var oldPassword = "old";
            var newPassword = "new";

            var user = new User
            {
                Id = userId,
                PasswordHash = "hashed_old"
            };

            var passwordDto = new ChangePasswordDto
            {
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _bcryptMock.Setup(b => b.VerifyPassword(oldPassword, "hashed_old")).Returns(true);
            _bcryptMock.Setup(b => b.HashPassword(newPassword)).Returns("hashed_new");
            _userRepoMock.Setup(r => r.Update(userId, It.IsAny<User>())).ReturnsAsync(user);

            var result = await _userService.UpdateUserPassword(userId, passwordDto);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void UpdateUserPassword_ShouldThrow_WhenOldPasswordInvalid()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, PasswordHash = "wrong_hash" };

            var passwordDto = new ChangePasswordDto
            {
                OldPassword = "wrong",
                NewPassword = "new"
            };

            _userRepoMock.Setup(r => r.Get(userId)).ReturnsAsync(user);
            _bcryptMock.Setup(b => b.VerifyPassword("wrong", "wrong_hash")).Returns(false);

            Assert.ThrowsAsync<InvalidOperationException>(() => _userService.UpdateUserPassword(userId, passwordDto));
        }
    }
}
