using DSS.Contexts;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class DocumentViewRepository : Repository<Guid, DocumentView>
    {
        private readonly ILogger<DocumentViewRepository> _logger;
        public DocumentViewRepository(DssContext context, ILogger<DocumentViewRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public override async Task<DocumentView> Get(Guid key)
        {
            try
            {
                var documentView = await _dssContext.DocumentViews.SingleOrDefaultAsync(u => u.Id == key);
                if (documentView == null)
                {
                    _logger.LogWarning("DOcument View not found with ID: {DocumentViewId}", key);
                    throw new KeyNotFoundException("No Document viewed with this id");
                }

                _logger.LogInformation("Fetched DocumentView with ID: {DocumentViewId}", key);
                return documentView;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching DocumentView with ID: {DocumentViewId}", key);
                throw;
            }
        }

        public override async Task<IEnumerable<DocumentView>> GetAll()
        {
            try
            {
                var documentViews = await _dssContext.DocumentViews.ToListAsync();
                _logger.LogInformation("Fetched all document views Count: {Count}", documentViews.Count);
                return documentViews;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching all document views");
                throw;
            }
        }
    }
}