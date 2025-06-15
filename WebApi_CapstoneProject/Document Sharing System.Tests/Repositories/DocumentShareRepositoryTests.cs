using DSS.Contexts;
using DSS.Models;
using DSS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSS.Tests.Repositories
{
    [TestFixture]
    public class DocumentShareRepositoryTests
    {
        private DssContext _context;
        private DocumentShareRepository _repository;
        private Mock<ILogger<DocumentViewRepository>> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<DssContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DssContext(options);
            _mockLogger = new Mock<ILogger<DocumentViewRepository>>();
            _repository = new DocumentShareRepository(_context, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Get_WhenDocumentShareExists_ReturnsDocumentShare()
        {
            // Arrange
            var docShare = new DocumentShare
            {
                Id = Guid.NewGuid(),
                DocumentId = Guid.NewGuid(),
                SharedWithUserId = Guid.NewGuid(),
                IsRevoked = false
            };
            _context.DocumentShares.Add(docShare);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.Get(docShare.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(docShare.Id));
        }

        [Test]
        public void Get_WhenDocumentShareDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.Get(id));
            Assert.That(ex.Message, Is.EqualTo("No Document shared with this id"));
        }

        [Test]
        public async Task GetAll_WhenCalled_ReturnsAllDocumentShares()
        {
            // Arrange
            _context.DocumentShares.AddRange(new[]
            {
                new DocumentShare { Id = Guid.NewGuid(), DocumentId = Guid.NewGuid(), SharedWithUserId = Guid.NewGuid(), IsRevoked = false },
                new DocumentShare { Id = Guid.NewGuid(), DocumentId = Guid.NewGuid(), SharedWithUserId = Guid.NewGuid(), IsRevoked = true }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}
