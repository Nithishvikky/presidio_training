using DSS.Controllers;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;


namespace DSS.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Test]
        public async Task PostUser_ValidInput_ReturnsCreatedResult()
        {
            // Arrange
            var userRequest = new UserAddRequestDto { Email = "test@example.com", Password = "123456" };
            var expectedUser = new User { Id = Guid.NewGuid(), Email = "test@example.com" };

            _userServiceMock.Setup(s => s.AddUser(userRequest)).ReturnsAsync(expectedUser);

            // Act
            var actionResult = await _controller.PostUser(userRequest);

            // Assert
            Assert.That(actionResult.Result, Is.TypeOf<CreatedResult>());

            var createdResult = actionResult.Result as CreatedResult;

            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.Value, Is.EqualTo(expectedUser));
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public async Task GetUser_ValidEmail_ReturnsOkResult()
        {
            // Arrange
            var email = "user@example.com";
            var expectedUser = new User { Id = Guid.NewGuid(), Email = email };
            _userServiceMock.Setup(s => s.GetUserByEmail(email)).ReturnsAsync(expectedUser);

            // Act
            var actionResult = await _controller.GetUser(email);

            // Assert
            Assert.That(actionResult.Result, Is.TypeOf<OkObjectResult>());

            var result = actionResult.Result as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(expectedUser));
        }

        [Test]
        public async Task ChangePassword_ValidRequest_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var passwordDto = new ChangePasswordDto { OldPassword = "old", NewPassword = "new" };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _userServiceMock.Setup(s => s.UpdateUserPassword(userId, passwordDto)).ReturnsAsync(new User());

            // Act
            var result = await _controller.ChangePassword(passwordDto) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo("Password updated successfully"));
        }

        [Test]
        public async Task ChangePassword_UnauthenticatedUser_ReturnsUnauthorized()
        {
            // Arrange
            var passwordDto = new ChangePasswordDto { OldPassword = "old", NewPassword = "new" };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // no user
            };

            // Act
            var result = await _controller.ChangePassword(passwordDto) as UnauthorizedObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }
    }
}
