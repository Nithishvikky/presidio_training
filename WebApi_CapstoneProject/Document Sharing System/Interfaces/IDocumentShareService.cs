using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IDocumentShareService
    {
        public Task<DocumentShare> GrantPermission(string fileName, string UploaderEmail, string sharedWithUserEmail);
        public Task<ICollection<DocumentShare>> GrantAllPermission(string fileName, string UploaderEmail);
        public Task<bool> IsDocumentSharedWithUser(Guid documentId, Guid SharedWithUserId);
        public Task<DocumentShare> RevokePermission(string fileName, string UploaderEmail, string sharedWithUserEmail);
        public Task RevokeAllPermission(string fileName, string UploaderEmail);

        public Task<ICollection<SharedResponseeDto>> GetSharedUsersByFileName(string fileName, string UploaderEmail);
        public Task<ICollection<UserDocDetailDto>> GetFilesSharedWithUser(Guid userId);

        public Task<DashboardDto> GetDashboard();
        public Task<ICollection<TopSharedDocumentDto>> GetTopSharedDocumentsAsync(int top);
    }
}