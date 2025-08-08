using Microsoft.AspNetCore.Mvc;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DSS.Controllers
{
    [ApiController]
    [Route("api/v1/user-activity-logs")]
    [Authorize]
    public class UserActivityLogController : ControllerBase
    {
        private readonly IUserActivityLogService _activityLogService;
        private readonly ILogger<UserActivityLogController> _logger;

        public UserActivityLogController(IUserActivityLogService activityLogService, ILogger<UserActivityLogController> logger)
        {
            _activityLogService = activityLogService;
            _logger = logger;
        }

        [HttpPost("LogActivity")]
        public async Task<ActionResult<UserActivityLogDto>> LogActivity([FromBody] CreateActivityLogDto activityDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 400,
                    ErrorMessage = string.Join(" | ", errorMessages)
                });
            }

            try
            {
                var activityLog = await _activityLogService.LogActivityAsync(activityDto);
                return Ok(new ApiResponse<UserActivityLogDto>
                {
                    Success = true,
                    Data = activityLog
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to log activity"
                });
            }
        }

        [HttpGet("GetMyActivityLogs")]
        public async Task<ActionResult<IEnumerable<UserActivityLogDto>>> GetMyActivityLogs(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var activityLogs = await _activityLogService.GetUserActivityLogsAsync(Guid.Parse(userId), pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserActivityLogDto>>
                {
                    Success = true,
                    Data = activityLogs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user activity logs");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user activity logs"
                });
            }
        }

        [HttpGet("GetUserActivityLogs/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserActivityLogDto>>> GetUserActivityLogs(Guid userId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var activityLogs = await _activityLogService.GetUserActivityLogsAsync(userId, pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserActivityLogDto>>
                {
                    Success = true,
                    Data = activityLogs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity logs for user: {UserId}", userId);
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user activity logs"
                });
            }
        }

        [HttpGet("GetAllActivityLogs")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserActivityLogDto>>> GetAllActivityLogs(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var activityLogs = await _activityLogService.GetAllActivityLogsAsync(pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserActivityLogDto>>
                {
                    Success = true,
                    Data = activityLogs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all activity logs");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get all activity logs"
                });
            }
        }

        [HttpGet("GetMyActivitySummary")]
        public async Task<ActionResult<UserActivitySummaryDto>> GetMyActivitySummary()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var summary = await _activityLogService.GetUserActivitySummaryAsync(Guid.Parse(userId));
                return Ok(new ApiResponse<UserActivitySummaryDto>
                {
                    Success = true,
                    Data = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user activity summary");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user activity summary"
                });
            }
        }

        [HttpGet("GetUserActivitySummary/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserActivitySummaryDto>> GetUserActivitySummary(Guid userId)
        {
            try
            {
                var summary = await _activityLogService.GetUserActivitySummaryAsync(userId);
                return Ok(new ApiResponse<UserActivitySummaryDto>
                {
                    Success = true,
                    Data = summary
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity summary for user: {UserId}", userId);
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user activity summary"
                });
            }
        }

        [HttpGet("GetAllUsersActivitySummary")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserActivitySummaryDto>>> GetAllUsersActivitySummary()
        {
            try
            {
                var summaries = await _activityLogService.GetAllUsersActivitySummaryAsync();
                return Ok(new ApiResponse<IEnumerable<UserActivitySummaryDto>>
                {
                    Success = true,
                    Data = summaries
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all users activity summary");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get all users activity summary"
                });
            }
        }

        [HttpGet("GetActivityLogsByType/{activityType}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserActivityLogDto>>> GetActivityLogsByType(string activityType, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var activityLogs = await _activityLogService.GetActivityLogsByTypeAsync(activityType, pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserActivityLogDto>>
                {
                    Success = true,
                    Data = activityLogs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity logs by type: {ActivityType}", activityType);
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get activity logs by type"
                });
            }
        }

        [HttpGet("GetActivityLogsByDateRange")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserActivityLogDto>>> GetActivityLogsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            int pageNumber = 1, 
            int pageSize = 50)
        {
            try
            {
                var activityLogs = await _activityLogService.GetActivityLogsByDateRangeAsync(startDate, endDate, pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserActivityLogDto>>
                {
                    Success = true,
                    Data = activityLogs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity logs by date range");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get activity logs by date range"
                });
            }
        }

        [HttpGet("GetMyActivityCount")]
        public async Task<ActionResult<int>> GetMyActivityCount()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var count = await _activityLogService.GetUserActivityCountAsync(Guid.Parse(userId));
                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Data = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user activity count");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user activity count"
                });
            }
        }
    }
} 