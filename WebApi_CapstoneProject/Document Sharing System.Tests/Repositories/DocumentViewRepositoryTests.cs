using DSS.Contexts;
using DSS.Models;
using DSS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;


namespace DSS.Tests.Repositories
{
    [TestFixture]
    public class DocumentViewRepositoryTests
    {
        private DssContext _context;
        private DocumentViewRepository _documentViewRepository;
        private Mock<ILogger<DocumentViewRepository>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DssContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DssContext(options);
            _loggerMock = new Mock<ILogger<DocumentViewRepository>>();
            _documentViewRepository = new DocumentViewRepository(_context, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_WhenDocumentViewExists_ReturnsDocumentView()
        {
            // Arrange
            var id = Guid.NewGuid();
            var view = new DocumentView
            {
                Id = id,
                ViewedAt = DateTime.UtcNow,
                ViewedByUserId = Guid.NewGuid(),
                DocumentId = Guid.NewGuid()
            };

            _context.DocumentViews.Add(view);
            await _context.SaveChangesAsync();

            // Act
            var result = await _documentViewRepository.Get(id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(id));
        }

        [Test]
        public void Get_WhenDocumentViewNotExists_ThrowsKeyNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _documentViewRepository.Get(invalidId);
            });

            Assert.That(ex.Message, Is.EqualTo("No Document viewed with this id"));
        }

        [Test]
        public async Task GetAll_WhenDocumentViewsExist_ReturnsAll()
        {
            // Arrange
            var views = new List<DocumentView>
            {
                new DocumentView { Id = Guid.NewGuid(), ViewedByUserId = Guid.NewGuid(), DocumentId = Guid.NewGuid(), ViewedAt = DateTime.UtcNow },
                new DocumentView { Id = Guid.NewGuid(), ViewedByUserId = Guid.NewGuid(), DocumentId = Guid.NewGuid(), ViewedAt = DateTime.UtcNow }
            };

            _context.DocumentViews.AddRange(views);
            await _context.SaveChangesAsync();

            // Act
            var result = await _documentViewRepository.GetAll();

            // Assert
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetAll_WhenNoDocumentViewsExist_ReturnsEmpty()
        {
            // Act
            var result = await _documentViewRepository.GetAll();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
