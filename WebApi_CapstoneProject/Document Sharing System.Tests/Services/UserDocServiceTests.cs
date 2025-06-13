using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Models;
using DSS.Services;
using Microsoft.Extensions.Logging;
using Moq;


namespace DSS.Tests.Services
{
    [TestFixture]
    public class UserDocServiceTests
    {
        private Mock<IRepository<Guid, UserDocument>> _userDocRepoMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<UserDocService>> _loggerMock;
        private UserDocService _userDocService;

        [SetUp]
        public void Setup()
        {
            _userDocRepoMock = new Mock<IRepository<Guid, UserDocument>>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UserDocService>>();

            _userDocService = new UserDocService(_userDocRepoMock.Object, _userServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task UploadDoc_ShouldAddAndReturnDocument()
        {
            var doc = new UserDocument { Id = Guid.NewGuid(), FileName = "file.pdf", UploadedById = Guid.NewGuid() };
            _userDocRepoMock.Setup(r => r.Add(doc)).ReturnsAsync(doc);

            var result = await _userDocService.UploadDoc(doc);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileName, Is.EqualTo("file.pdf"));
        }

        [Test]
        public async Task GetAllUserDocs_ShouldReturnDocuments()
        {
            var userId = Guid.NewGuid();
            var docs = new List<UserDocument> { new UserDocument { UploadedById = userId, IsDeleted = false } };

            _userDocRepoMock.Setup(r => r.GetAll()).ReturnsAsync(docs);

            var result = await _userDocService.GetAllUserDocs(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetAllUserDocs_ShouldThrow_WhenNoDocsFound()
        {
            var userId = Guid.NewGuid();
            var docs = new List<UserDocument>();
            _userDocRepoMock.Setup(r => r.GetAll()).ReturnsAsync(docs);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _userDocService.GetAllUserDocs(userId);
            });
        }

        [Test]
        public async Task GetByFileName_ShouldReturnDocument()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "test@email.com" };
            var doc = new UserDocument { UploadedById = user.Id, FileName = "abc.pdf", IsDeleted = false };

            _userServiceMock.Setup(s => s.GetUserByEmail(user.Email)).ReturnsAsync(user);
            _userDocRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<UserDocument> { doc });

            var result = await _userDocService.GetByFileName("abc.pdf", user.Email);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.FileName, Is.EqualTo("abc.pdf"));
        }

        [Test]
        public async Task DeleteByFileName_ShouldMarkAsDeleted()
        {
            var user = new User { Id = Guid.NewGuid() };
            var doc = new UserDocument { Id = Guid.NewGuid(), UploadedById = user.Id, FileName = "abc.pdf", IsDeleted = false };

            _userServiceMock.Setup(s => s.GetUserById(user.Id)).ReturnsAsync(user);
            _userDocRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<UserDocument> { doc });
            _userDocRepoMock.Setup(r => r.Update(doc.Id, It.IsAny<UserDocument>())).ReturnsAsync(doc);

            var result = await _userDocService.DeleteByFileName("abc.pdf", user.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsDeleted, Is.True);
        }
    }
}
