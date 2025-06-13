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
    public class DocumentViewControllerTests
    {
        private Mock<IDocumentViewService> _mockDocViewService;
        private DocumentViewController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockDocViewService = new Mock<IDocumentViewService>();
            _controller = new DocumentViewController(_mockDocViewService.Object);
        }

        private void SetUserWithId(Guid userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        // [Test]
        // public async Task GetMyViewHistory_WithValidUserId_ReturnsOkWithDocumentViews()
        // {
        //     // Arrange
        //     var userId = Guid.NewGuid();
        //     SetUserWithId(userId);

        //     var views = new List<DocumentView>
        //     {
        //         new DocumentView { Id = Guid.NewGuid(), ViewedByUserId = userId, ViewedAt = DateTime.UtcNow },
        //         new DocumentView { Id = Guid.NewGuid(), ViewedByUserId = userId, ViewedAt = DateTime.UtcNow }
        //     };

        //     _mockDocViewService
        //         .Setup(s => s.GetUserViewHistory(userId))
        //         .ReturnsAsync(views);

        //     // Act
        //     var result = await _controller.GetMyViewHistory();

        //     // Assert
        //     Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        //     var okResult = result.Result as OkObjectResult;
        //     Assert.That(okResult!.Value, Is.EqualTo(views));
        // }

        // [Test]
        // public async Task GetMyViewHistory_WithoutUserId_ReturnsUnauthorized()
        // {
        //     //Arrange
        //     _controller.ControllerContext = new ControllerContext()
        //     {
        //         HttpContext = new DefaultHttpContext()
        //     };

        //     // Act
        //     var result = await _controller.GetMyViewHistory();

        //     // Assert
        //     Assert.That(result.Result, Is.TypeOf<UnauthorizedObjectResult>());
        //     var unauthorized = result.Result as UnauthorizedObjectResult;
        //     Assert.That(((ErrorObjectDto)unauthorized!.Value!).ErrorNumber, Is.EqualTo(401));
        // }
    }
}
