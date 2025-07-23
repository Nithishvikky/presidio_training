using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface IUserActivityLogService
    {
        Task<UserActivityLogDto> LogActivityAsync(CreateActivityLogDto activityDto);
        Task<IEnumerable<UserActivityLogDto>> GetUserActivityLogsAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
        Task<IEnumerable<UserActivityLogDto>> GetAllActivityLogsAsync(int pageNumber = 1, int pageSize = 50);
        Task<UserActivitySummaryDto> GetUserActivitySummaryAsync(Guid userId);
        Task<IEnumerable<UserActivitySummaryDto>> GetAllUsersActivitySummaryAsync();
        Task<int> GetUserActivityCountAsync(Guid userId);
        Task<IEnumerable<UserActivityLogDto>> GetActivityLogsByTypeAsync(string activityType, int pageNumber = 1, int pageSize = 50);
        Task<IEnumerable<UserActivityLogDto>> GetActivityLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50);
    }
} 