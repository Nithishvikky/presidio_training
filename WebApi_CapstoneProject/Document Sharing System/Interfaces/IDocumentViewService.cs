using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IDocumentViewService
    {
        public Task<DocumentView> LogDocumentView(Guid documentId, Guid userId);
        public Task<IEnumerable<ViewerResponseDto>> GetUserViewHistory(Guid userId);
        public Task<IEnumerable<ViewerResponseDto>> GetViewerHistoryByFileName(Guid userId, string FileName);
    }
}