using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Misc;
using DSS.Models;
using DSS.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;


namespace DSS.Tests.Services
{
    [TestFixture]
    public class DocumentViewServiceTests
    {
        private Mock<IRepository<Guid, DocumentView>> _docViewRepoMock;
        private Mock<IRepository<Guid, UserDocument>> _userDocRepoMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IHubContext<NotificationHub>> _hubMock;
        private Mock<IClientProxy> _clientProxyMock;
        private Mock<ILogger<DocumentViewService>> _loggerMock;
        private DocumentViewService _service;

        [SetUp]
        public void SetUp()
        {
            _docViewRepoMock = new Mock<IRepository<Guid, DocumentView>>();
            _userDocRepoMock = new Mock<IRepository<Guid, UserDocument>>();
            _userServiceMock = new Mock<IUserService>();
            _hubMock = new Mock<IHubContext<NotificationHub>>();
            _clientProxyMock = new Mock<IClientProxy>();
            _loggerMock = new Mock<ILogger<DocumentViewService>>();

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
            _hubMock.Setup(h => h.Clients).Returns(mockClients.Object);

            _service = new DocumentViewService(
                _docViewRepoMock.Object,
                _hubMock.Object,
                _userDocRepoMock.Object,
                _userServiceMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task LogDocumentView_ShouldLogAndSendNotification()
        {
            var docId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var view = new DocumentView { Id = Guid.NewGuid(), DocumentId = docId, ViewedByUserId = userId, ViewedAt = DateTime.UtcNow };
            var document = new UserDocument { Id = docId, UploadedById = Guid.NewGuid(), FileName = "file.pdf" };
            var viewer = new User { Id = userId, Username = "Viewer" };
            var uploader = new User { Id = document.UploadedById, Email = "uploader@example.com" };

            _docViewRepoMock.Setup(r => r.Add(It.IsAny<DocumentView>())).ReturnsAsync(view);
            _userDocRepoMock.Setup(r => r.Get(docId)).ReturnsAsync(document);
            _userServiceMock.Setup(s => s.GetUserById(userId)).ReturnsAsync(viewer);
            _userServiceMock.Setup(s => s.GetUserById(document.UploadedById)).ReturnsAsync(uploader);

            var result = await _service.LogDocumentView(docId, userId);

            Assert.That(result, Is.Not.Null);
            _clientProxyMock.Verify(c => c.SendCoreAsync("DocumentViewed", It.IsAny<object[]>(), default), Times.Once);
        }

        // [Test]
        // public async Task GetUserViewHistory_ShouldReturnViews()
        // {
        //     var userId = Guid.NewGuid();
        //     var views = new List<DocumentView> {
        //         new DocumentView { Id = Guid.NewGuid(), ViewedByUserId = userId },
        //         new DocumentView { Id = Guid.NewGuid(), ViewedByUserId = userId }
        //     };

        //     _docViewRepoMock.Setup(r => r.GetAll()).ReturnsAsync(views);

        //     var result = await _service.GetUserViewHistory(userId);

        //     Assert.That(result.Count(), Is.EqualTo(2));
        // }

        // [Test]
        // public void GetUserViewHistory_ShouldThrow_WhenNoViews()
        // {
        //     var userId = Guid.NewGuid();
        //     _docViewRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<DocumentView>());

        //     Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetUserViewHistory(userId));
        // }
    }
}
