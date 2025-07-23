using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IUserRequestService
    {
        Task<UserRequestDto> CreateRequestAsync(CreateUserRequestDto requestDto, Guid userId);
        Task<UserRequestDto> ProcessRequestAsync(ProcessUserRequestDto processDto, Guid adminUserId);
        Task<IEnumerable<UserRequestDto>> GetUserRequestsAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<UserRequestDto>> GetAllRequestsAsync(int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<UserRequestDto>> GetPendingRequestsAsync(int pageNumber = 1, int pageSize = 50);
        Task<UserRequestDto> GetRequestByIdAsync(Guid requestId);
        Task<DocumentAccessStatusDto> GetDocumentAccessStatusAsync(Guid documentId, Guid userId);
        Task<IEnumerable<DocumentAccessStatusDto>> GetUserAccessibleDocumentsAsync(Guid userId);
        Task<bool> HasDocumentAccessAsync(Guid documentId, Guid userId);
    }
} 