using Microsoft.AspNetCore.Mvc;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DSS.Controllers
{
    [ApiController]
    [Route("api/v1/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        private readonly IInactiveUserNotificationService _inactiveUserNotificationService;


        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger,IInactiveUserNotificationService inactiveUserNotificationService)
        {
            _notificationService = notificationService;
            _logger = logger;
            _inactiveUserNotificationService = inactiveUserNotificationService;
        }

        [HttpPost("CreateNotification")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] CreateNotificationDto notificationDto)
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
                var notification = await _notificationService.CreateNotification(notificationDto);
                return Ok(new ApiResponse<Notification>
                {
                    Success = true,
                    Data = notification
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create notification");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to create notification"
                });
            }
        }

        [HttpGet("GetUserNotifications")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications(bool? isRead = null)
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

                var notifications = await _notificationService.GetUserNotifications(Guid.Parse(userId), isRead);
                return Ok(new ApiResponse<IEnumerable<NotificationDto>>
                {
                    Success = true,
                    Data = notifications
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user notifications");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user notifications"
                });
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotificationsWithPaging(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isRead = null)
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

                var notifications = await _notificationService.GetUserNotifications(Guid.Parse(userId), isRead);
                
                // Apply pagination
                var totalCount = notifications.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var pagedNotifications = notifications
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new ApiResponse<IEnumerable<NotificationDto>>
                {
                    Success = true,
                    Data = pagedNotifications
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user notifications with paging");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user notifications"
                });
            }
        }

        [HttpPut("MarkAsRead")]
        public async Task<ActionResult<NotificationDto>> MarkNotificationAsRead([FromBody] MarkNotificationReadDto request)
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var notification = await _notificationService.MarkNotificationAsRead(Guid.Parse(userId), request.NotificationId);
                return Ok(new ApiResponse<NotificationDto>
                {
                    Success = true,
                    Data = notification
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorObjectDto
                {
                    ErrorNumber = 404,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notification as read");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to mark notification as read"
                });
            }
        }

        [HttpPut("{notificationId}/read")]
        public async Task<ActionResult<NotificationDto>> MarkNotificationAsReadById(Guid notificationId)
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

                var notification = await _notificationService.MarkNotificationAsRead(Guid.Parse(userId), notificationId);
                return Ok(new ApiResponse<NotificationDto>
                {
                    Success = true,
                    Data = notification
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorObjectDto
                {
                    ErrorNumber = 404,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notification as read by ID");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to mark notification as read"
                });
            }
        }

        [HttpPut("MarkAllAsRead")]
        public async Task<ActionResult> MarkAllNotificationsAsRead()
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

                var result = await _notificationService.MarkAllNotificationsAsRead(Guid.Parse(userId));
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = "All notifications marked as read successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all notifications as read");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to mark all notifications as read"
                });
            }
        }

        [HttpPut("user/read-all")]
        public async Task<ActionResult> MarkAllUserNotificationsAsRead()
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

                var result = await _notificationService.MarkAllNotificationsAsRead(Guid.Parse(userId));
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = "All notifications marked as read successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all user notifications as read");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to mark all notifications as read"
                });
            }
        }

        [HttpGet("UnreadCount")]
        public async Task<ActionResult<int>> GetUnreadNotificationCount()
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

                var count = await _notificationService.GetUnreadNotificationCount(Guid.Parse(userId));
                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Data = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread notification count");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get unread notification count"
                });
            }
        }

        [HttpGet("user/unread-count")]
        public async Task<ActionResult<int>> GetUserUnreadNotificationCount()
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

                var count = await _notificationService.GetUnreadNotificationCount(Guid.Parse(userId));
                return Ok(new ApiResponse<int>
                {
                    Success = true,
                    Data = count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user unread notification count");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get unread notification count"
                });
            }
        }

        [HttpDelete("DeleteNotification/{notificationId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteNotification(Guid notificationId)
        {
            try
            {
                var result = await _notificationService.DeleteNotification(notificationId);
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = "Notification deleted successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorObjectDto
                {
                    ErrorNumber = 404,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete notification");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to delete notification"
                });
            }
        }

        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> DeleteUserNotification(Guid notificationId)
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

                // Note: This endpoint allows users to delete their own notifications
                // You might want to add validation to ensure the notification belongs to the user
                var result = await _notificationService.DeleteNotification(notificationId);
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = "Notification deleted successfully"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorObjectDto
                {
                    ErrorNumber = 404,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user notification");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to delete notification"
                });
            }
        }

        [HttpPost("TriggerInactiveUserCheck")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> TriggerInactiveUserCheck()
        {
            try
            {
                var inactiveUserNotificationService = HttpContext.RequestServices.GetRequiredService<IInactiveUserNotificationService>();
                await inactiveUserNotificationService.NotifyInactiveUsersAsync();
                
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = "Inactive user notification check completed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to trigger inactive user check");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to trigger inactive user check"
                });
            }
        }

        [HttpPost("NotifyInactiveUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> NotifyInactiveUsers()
        {
            await _inactiveUserNotificationService.NotifyInactiveUsersAsync();
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data = $"Notifications sent to users inactive for more than {30} days."
            });
        }
    }
} 