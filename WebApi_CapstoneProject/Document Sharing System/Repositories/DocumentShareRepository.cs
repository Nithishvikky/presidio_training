using DSS.Contexts;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class DocumentShareRepository : Repository<Guid, DocumentShare>
    {
        private readonly ILogger<DocumentViewRepository> _logger;
        public DocumentShareRepository(DssContext context, ILogger<DocumentViewRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public override async Task<DocumentShare> Get(Guid key)
        {
            try
            {
                var documentShare = await _dssContext.DocumentShares.SingleOrDefaultAsync(u => u.Id == key);
                if (documentShare == null)
                {
                    _logger.LogWarning("Document share not found with ID: {DocumentShareId}", key);
                    throw new KeyNotFoundException("No Document shared with this id");
                }

                return documentShare;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching DocumentShare with ID: {DocumentShareId}", key);
                throw;
            }
        }

        public override async Task<IEnumerable<DocumentShare>> GetAll()
        {
            try
            {
                var documentShares = await _dssContext.DocumentShares.ToListAsync();
                _logger.LogInformation("Fetched all document shares Count: {Count}", documentShares.Count);
                return documentShares;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching all document shares");
                throw;
            }
        }
    }
}