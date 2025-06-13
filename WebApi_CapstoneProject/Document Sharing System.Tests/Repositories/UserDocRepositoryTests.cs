using DSS.Contexts;
using DSS.Models;
using DSS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DSS.Tests.Repositories
{
    [TestFixture]
    public class UserDocRepositoryTests
    {
        private DssContext _context;
        private UserDocRepository _userDocRepository;
        private Mock<ILogger<UserDocRepository>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DssContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DssContext(options);
            _loggerMock = new Mock<ILogger<UserDocRepository>>();
            _userDocRepository = new UserDocRepository(_context, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Get_WhenDocumentExists_ReturnsDocument()
        {
            // Arrange
            var docId = Guid.NewGuid();
            var doc = new UserDocument
            {
                Id = docId,
                FileName = "sample.pdf"
            };

            _context.UserDocuments.Add(doc);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userDocRepository.Get(docId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(docId));
            Assert.That(result.FileName, Is.EqualTo("sample.pdf"));
        }

        [Test]
        public void Get_WhenDocumentNotExists_ThrowsKeyNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            {
                await _userDocRepository.Get(invalidId);
            });

            Assert.That(ex.Message, Is.EqualTo("No document found"));
        }

        [Test]
        public async Task GetAll_WhenDocumentsExist_ReturnsAll()
        {
            // Arrange
            var docs = new List<UserDocument>
            {
                new UserDocument { Id = Guid.NewGuid(), FileName = "doc1.pdf" },
                new UserDocument { Id = Guid.NewGuid(), FileName = "doc2.pdf" }
            };

            _context.UserDocuments.AddRange(docs);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userDocRepository.GetAll();

            // Assert
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetAll_WhenNoDocuments_ReturnsEmpty()
        {
            // Act
            var result = await _userDocRepository.GetAll();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
