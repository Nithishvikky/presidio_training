using DSS.Controllers;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DSS.Tests.Controllers
{
    [TestFixture]
    public class UserDocControllerTests
    {
        private Mock<IUserDocService> _mockUserDocService;
        private Mock<IDocumentViewService> _mockDocViewService;
        private UserDocController _controller;
        private Mock<IDocumentShareService> _mockDocShareService;

        [SetUp]
        public void Setup()
        {
            _mockUserDocService = new Mock<IUserDocService>();
            _mockDocViewService = new Mock<IDocumentViewService>();
            _mockDocShareService = new Mock<IDocumentShareService>();
            _controller = new UserDocController(_mockUserDocService.Object, _mockDocViewService.Object,_mockDocShareService.Object);
        }

        private void SetUser(Guid userId, string email = "user@example.com")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Test]
        public async Task UploadDocument_FileIsNull_ReturnsBadRequest()
        {
            var result = await _controller.UploadDocument(null);
            Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UploadDocument_ValidFile_ReturnsCreated()
        {
            var fileMock = new Mock<IFormFile>();
            var content = "Hello world";
            var fileName = "test.txt";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/plain");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns<Stream, CancellationToken>((s, ct) =>
                    {
                        stream.CopyTo(s);
                        return Task.CompletedTask;
                    });

            var userId = Guid.NewGuid();
            SetUser(userId);

            _mockUserDocService.Setup(s => s.UploadDoc(It.IsAny<UserDocument>()))
                .ReturnsAsync(new UserDocument { FileName = fileName });

            var result = await _controller.UploadDocument(fileMock.Object);
            Assert.That(result.Result, Is.TypeOf<CreatedResult>());
        }

        [Test]
        public async Task DownloadFile_ValidUser_ReturnsFile()
        {
            var fileName = "file.txt";
            var uploader = "email@example.com";
            var userId = Guid.NewGuid();
            SetUser(userId);

            var mockDocument = new UserDocument
            {
                FileName = fileName,
                ContentType = "text/plain",
                FileData = Encoding.UTF8.GetBytes("data"),
                Id = Guid.NewGuid()
            };
            
            _mockUserDocService.Setup(s => s.GetByFileName(fileName, uploader))
                .ReturnsAsync(mockDocument);
            _mockDocShareService.Setup(s => s.IsDocumentSharedWithUser(mockDocument.Id, userId))
                .ReturnsAsync(true);

            var result = await _controller.DownloadFile(fileName, uploader);
            Assert.That(result, Is.TypeOf<FileContentResult>());
        }

        [Test]
        public async Task MyDocumentDetails_ReturnsDocuments()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _mockUserDocService.Setup(s => s.GetAllUserDocs(userId))
                .ReturnsAsync(new List<UserDocument> {
                    new UserDocument { FileName = "test.txt" }
                });

            var result = await _controller.MyDocumentDetails();
            Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task DeleteMyDocument_ReturnsOk()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _mockUserDocService.Setup(s => s.DeleteByFileName("test.txt", userId))
                .ReturnsAsync(new UserDocument { FileName = "test.txt" });

            var result = await _controller.DeleteMyDocument("test.txt");
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}
