using DSS.Controllers;
using DSS.Interfaces;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace DSS.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService;
        private AuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Test]
        public async Task Login_WithValidModel_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "test@example.com", Password = "password" };
            var authResponse = new AuthResponse { AccessToken = "access", RefreshToken = "refresh" };

            _mockAuthService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(authResponse));
        }

        [Test]
        public async Task Login_WithInvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginRequestDto { Email = "", Password = "" };
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
            var badReq = result.Result as BadRequestObjectResult;
            Assert.That(((ErrorObjectDto)badReq!.Value!).ErrorNumber, Is.EqualTo(400));
        }

        [Test]
        public async Task Refresh_WithValidToken_ReturnsNewAuthTokens()
        {
            // Arrange
            var refreshToken = "valid-refresh";
            var authResponse = new AuthResponse { AccessToken = "new-access", RefreshToken = "new-refresh" };

            _mockAuthService.Setup(s => s.RefreshAsync(refreshToken)).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Refresh(refreshToken);

            // Assert
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
            var ok = result.Result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo(authResponse));
        }

        [Test]
        public async Task Logout_CallsServiceAndReturnsOk()
        {
            // Arrange
            var token = "some-refresh-token";
            _mockAuthService.Setup(s => s.LogoutAsync(token)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout(token);

            // Assert
            Assert.That(result, Is.TypeOf<OkObjectResult>());
            var ok = result as OkObjectResult;
            Assert.That(ok!.Value, Is.EqualTo("Logged out sucessfully"));
        }
    }
}
