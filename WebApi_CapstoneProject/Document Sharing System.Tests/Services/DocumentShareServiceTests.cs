using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using DSS.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSS.Tests.Services
{
    [TestFixture]
    public class DocumentShareServiceTests
    {
        private Mock<IRepository<Guid, DocumentShare>> _mockShareRepo;
        private Mock<IRepository<Guid, UserDocument>> _mockUserDocRepo;
        private Mock<ILogger<DocumentShareService>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IUserDocService> _mockUserDocService;
        private DocumentShareService _service;

        private Guid _docId;
        private Guid _userId;
        private UserDocument _mockDocument;
        private User _mockUser;

        [SetUp]
        public void Setup()
        {
            _mockShareRepo = new Mock<IRepository<Guid, DocumentShare>>();
            _mockUserDocRepo = new Mock<IRepository<Guid, UserDocument>>();
            _mockLogger = new Mock<ILogger<DocumentShareService>>();
            _mockUserService = new Mock<IUserService>();
            _mockUserDocService = new Mock<IUserDocService>();

            _service = new DocumentShareService(
                _mockShareRepo.Object,
                _mockLogger.Object,
                _mockUserService.Object,
                _mockUserDocService.Object,
                _mockUserDocRepo.Object
            );

            _docId = Guid.NewGuid();
            _userId = Guid.NewGuid();
            _mockDocument = new UserDocument { Id = _docId, FileName = "test.pdf" };
            _mockUser = new User { Id = _userId, Username = "user1" };
        }

        [Test]
        public async Task GrantPermission_ShouldReturnSharedDocument()
        {
            _mockUserDocService.Setup(x => x.GetByFileName("test.pdf", "uploader@test.com")).ReturnsAsync(_mockDocument);
            _mockUserService.Setup(x => x.GetUserByEmail("user1@test.com")).ReturnsAsync(_mockUser);
            _mockShareRepo.Setup(x => x.Add(It.IsAny<DocumentShare>())).ReturnsAsync((DocumentShare d) => d);

            var result = await _service.GrantPermission("test.pdf", "uploader@test.com", "user1@test.com");

            Assert.That(result.DocumentId, Is.EqualTo(_docId));
            Assert.That(result.SharedWithUserId, Is.EqualTo(_userId));
        }

        [Test]
        public async Task RevokePermission_ShouldSetIsRevokedToTrue()
        {
            var share = new DocumentShare { DocumentId = _docId, SharedWithUserId = _userId, IsRevoked = false, Id = Guid.NewGuid() };

            _mockUserDocService.Setup(x => x.GetByFileName("test.pdf", "uploader@test.com")).ReturnsAsync(_mockDocument);
            _mockUserService.Setup(x => x.GetUserByEmail("user1@test.com")).ReturnsAsync(_mockUser);
            _mockShareRepo.Setup(x => x.GetAll()).ReturnsAsync(new List<DocumentShare> { share });
            _mockShareRepo.Setup(x => x.Update(share.Id, It.IsAny<DocumentShare>())).ReturnsAsync((Guid id, DocumentShare s) => s);

            var result = await _service.RevokePermission("test.pdf", "uploader@test.com", "user1@test.com");

            Assert.That(result.IsRevoked, Is.True);
        }

        [Test]
        public async Task IsDocumentSharedWithUser_ShouldReturnTrue_WhenShareExists()
        {
            var share = new DocumentShare { DocumentId = _docId, SharedWithUserId = _userId, IsRevoked = false };
            _mockShareRepo.Setup(x => x.GetAll()).ReturnsAsync(new List<DocumentShare> { share });

            var result = await _service.IsDocumentSharedWithUser(_docId, _userId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetSharedUsersByFileName_ShouldReturnUsernames()
        {
            var share = new DocumentShare { DocumentId = _docId, SharedWithUserId = _userId, IsRevoked = false };

            _mockUserDocService.Setup(x => x.GetByFileName("test.pdf", "uploader@test.com")).ReturnsAsync(_mockDocument);
            _mockShareRepo.Setup(x => x.GetAll()).ReturnsAsync(new List<DocumentShare> { share });
            _mockUserService.Setup(x => x.GetUserById(_userId)).ReturnsAsync(_mockUser);

            var result = await _service.GetSharedUsersByFileName("test.pdf", "uploader@test.com");

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().UserName, Is.EqualTo("user1"));
        }

        [Test]
        public async Task GetFilesSharedWithUser_ShouldReturnFilenames()
        {
            var share = new DocumentShare { DocumentId = _docId, SharedWithUserId = _userId, IsRevoked = false };

            _mockUserService.Setup(x => x.GetUserById(_userId)).ReturnsAsync(_mockUser);
            _mockShareRepo.Setup(x => x.GetAll()).ReturnsAsync(new List<DocumentShare> { share });
            _mockUserDocRepo.Setup(x => x.Get(_docId)).ReturnsAsync(_mockDocument);

            var result = await _service.GetFilesSharedWithUser(_userId);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().FileName, Is.EqualTo("test.pdf"));
        }
    }
}
