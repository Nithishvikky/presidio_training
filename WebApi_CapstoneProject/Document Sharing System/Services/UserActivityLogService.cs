using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using DSS.Repositories;
using Microsoft.AspNetCore.SignalR;
using DSS.Misc;

namespace DSS.Services
{
    public class UserActivityLogService : IUserActivityLogService
    {
        private readonly IRepository<Guid, UserActivityLog> _activityLogRepository;
        private readonly UserActivityLogRepository _userActivityLogRepository;
        private readonly IUserService _userService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<UserActivityLogService> _logger;

        public UserActivityLogService(
            IRepository<Guid, UserActivityLog> activityLogRepository,
            UserActivityLogRepository userActivityLogRepository,
            IUserService userService,
            IHubContext<NotificationHub> hubContext,
            ILogger<UserActivityLogService> logger)
        {
            _activityLogRepository = activityLogRepository;
            _userActivityLogRepository = userActivityLogRepository;
            _userService = userService;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<UserActivityLogDto> LogActivityAsync(CreateActivityLogDto activityDto)
        {
            try
            {
                _logger.LogInformation("Logging activity for user: {UserId}, type: {ActivityType}", 
                    activityDto.UserId, activityDto.ActivityType);

                var activityLog = new UserActivityLog
                {
                    UserId = activityDto.UserId,
                    ActivityType = activityDto.ActivityType,
                    Description = activityDto.Description,
                    Timestamp = DateTime.UtcNow
                };

                var result = await _activityLogRepository.Add(activityLog);

                var activityLogDto = new UserActivityLogDto
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    UserEmail = string.Empty, // Will be populated when fetched with includes
                    UserUsername = string.Empty, // Will be populated when fetched with includes
                    ActivityType = result.ActivityType,
                    Description = result.Description,
                    Timestamp = result.Timestamp,
                };

                // Send real-time activity update
                try
                {
                    await _hubContext.Clients.Group(activityDto.UserId.ToString()).SendAsync("ReceiveActivityUpdate", activityLogDto);
                    _logger.LogInformation("Real-time activity update sent to user: {UserId}", activityDto.UserId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send real-time activity update to user: {UserId}", activityDto.UserId);
                }

                _logger.LogInformation("Activity logged successfully for user: {UserId}, type: {ActivityType}", 
                    activityDto.UserId, activityDto.ActivityType);

                return activityLogDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity for user: {UserId}", activityDto.UserId);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLogDto>> GetUserActivityLogsAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("Fetching activity logs for user: {UserId}, page: {PageNumber}, size: {PageSize}", 
                    userId, pageNumber, pageSize);

                var activityLogs = await _userActivityLogRepository.GetUserActivityLogs(userId, pageNumber, pageSize);

                var activityLogDtos = activityLogs.Select(al => new UserActivityLogDto
                {
                    Id = al.Id,
                    UserId = al.UserId,
                    UserEmail = al.User?.Email ?? string.Empty,
                    UserUsername = al.User?.Username ?? string.Empty,
                    ActivityType = al.ActivityType,
                    Description = al.Description,
                    Timestamp = al.Timestamp
                }).ToList();

                _logger.LogInformation("Retrieved {Count} activity logs for user: {UserId}", activityLogDtos.Count, userId);
                return activityLogDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity logs for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLogDto>> GetAllActivityLogsAsync(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching all activity logs, page: {PageNumber}, size: {PageSize}", pageNumber, pageSize);

                var allActivityLogs = await _activityLogRepository.GetAll();
                var pagedActivityLogs = allActivityLogs
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                var activityLogDtos = pagedActivityLogs.Select(al => new UserActivityLogDto
                {
                    Id = al.Id,
                    UserId = al.UserId,
                    UserEmail = al.User?.Email ?? string.Empty,
                    UserUsername = al.User?.Username ?? string.Empty,
                    ActivityType = al.ActivityType,
                    Description = al.Description,
                    Timestamp = al.Timestamp
                }).ToList();

                _logger.LogInformation("Retrieved {Count} activity logs", activityLogDtos.Count);
                return activityLogDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all activity logs");
                throw;
            }
        }

        public async Task<UserActivitySummaryDto> GetUserActivitySummaryAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting activity summary for user: {UserId}", userId);

                var user = await _userService.GetUserById(userId);
                var activityLogs = await _userActivityLogRepository.GetUserActivityLogs(userId, 1, int.MaxValue);

                var summary = new UserActivitySummaryDto
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    UserUsername = user.Username,
                    LastLogin = user.LastLogin,
                    LastActivity = activityLogs.Any() ? activityLogs.Max(al => al.Timestamp) : null,
                    TotalActivities = activityLogs.Count(),
                    LoginCount = activityLogs.Count(al => al.ActivityType == "Login"),
                    DocumentUploads = activityLogs.Count(al => al.ActivityType == "DocumentUpload"),
                    DocumentShares = activityLogs.Count(al => al.ActivityType == "DocumentShare")
                };

                _logger.LogInformation("Activity summary retrieved for user: {UserId}", userId);
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity summary for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivitySummaryDto>> GetAllUsersActivitySummaryAsync()
        {
            try
            {
                _logger.LogInformation("Getting activity summary for all users");

                var allUsers = await _userService.GetAllUsersOnly();
                var summaries = new List<UserActivitySummaryDto>();

                foreach (var user in allUsers)
                {
                    try
                    {
                        var summary = await GetUserActivitySummaryAsync(user.Id);
                        summaries.Add(summary);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get activity summary for user: {UserId}, skipping", user.Id);
                    }
                }

                _logger.LogInformation("Activity summary retrieved for {Count} users", summaries.Count);
                return summaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity summary for all users");
                throw;
            }
        }

        public async Task<int> GetUserActivityCountAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting activity count for user: {UserId}", userId);
                var count = await _userActivityLogRepository.GetUserActivityCount(userId);
                _logger.LogInformation("Activity count for user {UserId}: {Count}", userId, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity count for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLogDto>> GetActivityLogsByTypeAsync(string activityType, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching activity logs by type: {ActivityType}, page: {PageNumber}, size: {PageSize}", 
                    activityType, pageNumber, pageSize);

                var activityLogs = await _userActivityLogRepository.GetActivityLogsByType(activityType, pageNumber, pageSize);

                var activityLogDtos = activityLogs.Select(al => new UserActivityLogDto
                {
                    Id = al.Id,
                    UserId = al.UserId,
                    UserEmail = al.User?.Email ?? string.Empty,
                    UserUsername = al.User?.Username ?? string.Empty,
                    ActivityType = al.ActivityType,
                    Description = al.Description,
                    Timestamp = al.Timestamp
                }).ToList();

                _logger.LogInformation("Retrieved {Count} activity logs for type: {ActivityType}", activityLogDtos.Count, activityType);
                return activityLogDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity logs by type: {ActivityType}", activityType);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLogDto>> GetActivityLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching activity logs from {StartDate} to {EndDate}, page: {PageNumber}, size: {PageSize}", 
                    startDate, endDate, pageNumber, pageSize);

                var activityLogs = await _userActivityLogRepository.GetActivityLogsByDateRange(startDate, endDate, pageNumber, pageSize);

                var activityLogDtos = activityLogs.Select(al => new UserActivityLogDto
                {
                    Id = al.Id,
                    UserId = al.UserId,
                    UserEmail = al.User?.Email ?? string.Empty,
                    UserUsername = al.User?.Username ?? string.Empty,
                    ActivityType = al.ActivityType,
                    Description = al.Description,
                    Timestamp = al.Timestamp
                }).ToList();

                _logger.LogInformation("Retrieved {Count} activity logs for date range", activityLogDtos.Count);
                return activityLogDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity logs by date range");
                throw;
            }
        }
    }
} 