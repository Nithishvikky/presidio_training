using DSS.Controllers;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace DSS.Tests.Controllers
{
    [TestFixture]
    public class DocumentShareControllerTests
    {
        private Mock<IDocumentShareService> _mockShareService;
        private DocumentShareController _controller;

        [SetUp]
        public void Setup()
        {
            _mockShareService = new Mock<IDocumentShareService>();
            _controller = new DocumentShareController(_mockShareService.Object);
        }

        private void SetUserContext(string email = null, string userId = null, string role = "Admin")
        {
            var claims = new List<Claim>();
            if (email != null)
                claims.Add(new Claim(ClaimTypes.Email, email));
            if (userId != null)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            if (role != null)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };
        }

        [Test]
        public async Task PostShare_ReturnsCreated_WhenValid()
        {
            SetUserContext(email: "admin@example.com");
            var share = new DocumentShare { Id = Guid.NewGuid() };
            _mockShareService.Setup(x => x.GrantPermission("doc.pdf", "admin@example.com", "user@example.com")).ReturnsAsync(share);

            var result = await _controller.PostShare("doc.pdf", "user@example.com");

            Assert.That(result.Result, Is.TypeOf<CreatedResult>());
        }

        [Test]
        public async Task PostShare_ReturnsUnauthorized_WhenEmailMissing()
        {
            SetUserContext(email: null);

            var result = await _controller.PostShare("doc.pdf", "user@example.com");

            Assert.That(result.Result, Is.TypeOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task DeleteShare_ReturnsOk_WhenValid()
        {
            SetUserContext(email: "admin@example.com");
            var documentShare = new DocumentShare
            {
                Id = Guid.NewGuid(),
                IsRevoked = true
            };
            _mockShareService.Setup(x => x.RevokePermission("doc.pdf", "admin@example.com", "user@example.com")).ReturnsAsync(documentShare);

            var result = await _controller.DeleteShare("doc.pdf", "user@example.com");

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task FilesSharedUser_ReturnsOk_WithData()
        {
            var userId = Guid.NewGuid();
            SetUserContext(userId: userId.ToString());
            var data = new List<SharedResponseeDto> { new SharedResponseeDto { FileName = "file", UserName = "user" } };
            _mockShareService.Setup(x => x.GetFilesSharedWithUser(userId)).ReturnsAsync(data);

            var result = await _controller.FilesSharedUser();

            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task GetSharedUsers_ReturnsOk_WithData()
        {
            SetUserContext(email: "admin@example.com");
            var data = new List<SharedResponseeDto> { new SharedResponseeDto { FileName = "file", UserName = "user" } };
            _mockShareService.Setup(x => x.GetSharedUsersByFileName("doc.pdf", "admin@example.com")).ReturnsAsync(data);

            var result = await _controller.FilesSharedUser("doc.pdf");

            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        }
    }
}
