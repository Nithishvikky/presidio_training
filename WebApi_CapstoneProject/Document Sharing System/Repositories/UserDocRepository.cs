using DSS.Contexts;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class UserDocRepository : Repository<Guid, UserDocument>
    {
        private readonly ILogger<UserDocRepository> _logger;
        public UserDocRepository(DssContext context, ILogger<UserDocRepository> logger) : base(context)
        {
            _logger = logger;
        }
        public override async Task<UserDocument> Get(Guid key)
        {
            try
            {
                _logger.LogInformation("Attempting to fetch document with ID: {DocumentId}", key);
                var doc = await _dssContext.UserDocuments.SingleOrDefaultAsync(u => u.Id == key);
                
                if (doc == null)
                {
                    _logger.LogWarning("Document not found with ID: {DocumentId}", key);
                    throw new KeyNotFoundException("No document found");
                }
                
                _logger.LogInformation("Document found, fetching user with ID: {UserId}", doc.UploadedById);
                doc.UploadedByUser = await _dssContext.Users.SingleOrDefaultAsync(u => u.Id == doc.UploadedById);
                _logger.LogInformation("Fetched document with ID: {DocumentId}",key);
                return doc;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching document with ID: {DocumentId}",key);
                throw;
            }
        }

        public override async Task<IEnumerable<UserDocument>> GetAll()
        {
            try{
                var docs = await _dssContext.UserDocuments.ToListAsync();
                _logger.LogInformation("Fetched all documents Count: {Count}", docs.Count);
                return docs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all documents");
                throw;
            }
        }
    }
}