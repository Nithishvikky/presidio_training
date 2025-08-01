using DSS.Interfaces;
using DSS.Models;

namespace DSS.Services
{
    public class DocumentRearchiveService : IDocumentRearchiveService
    {
        private readonly IRepository<Guid, UserDocument> _userDocumentRepository;
        private readonly ILogger<DocumentRearchiveService> _logger;

        public DocumentRearchiveService(
            IRepository<Guid, UserDocument> userDocumentRepository,
            ILogger<DocumentRearchiveService> logger)
        {
            _userDocumentRepository = userDocumentRepository;
            _logger = logger;
        }

        public async Task RearchiveExpiredDocumentsAsync()
        {
            try
            {
                _logger.LogInformation("Starting document rearchive check");

                var allDocuments = await _userDocumentRepository.GetAll();
                var expiredDocuments = allDocuments
                    .Where(d => d.Status == "TemporarilyUnarchived" && 
                               d.ScheduledRearchiveAt.HasValue && 
                               d.ScheduledRearchiveAt.Value <= DateTime.UtcNow)
                    .ToList();

                if (!expiredDocuments.Any())
                {
                    _logger.LogInformation("No documents to rearchive");
                    return;
                }

                var rearchivedCount = 0;
                foreach (var document in expiredDocuments)
                {
                    try
                    {
                        document.Status = "Archived";
                        document.ArchivedAt = DateTime.UtcNow;
                        document.TemporarilyUnarchivedAt = null;
                        document.ScheduledRearchiveAt = null;

                        await _userDocumentRepository.Update(document.Id, document);
                        rearchivedCount++;

                        _logger.LogInformation("Document {DocumentId} ({FileName}) rearchived successfully", 
                            document.Id, document.FileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to rearchive document {DocumentId}", document.Id);
                    }
                }

                _logger.LogInformation("Rearchive process completed. {Count} documents rearchived out of {Total} expired documents", 
                    rearchivedCount, expiredDocuments.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during document rearchive process");
                throw;
            }
        }
    }
} 