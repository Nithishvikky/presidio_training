using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using DSS.Repositories;
using Microsoft.AspNetCore.SignalR;
using DSS.Misc;

namespace DSS.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Guid, Notification> _notificationRepository;
        private readonly UserNotificationRepository _userNotificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IRepository<Guid, Notification> notificationRepository,
            UserNotificationRepository userNotificationRepository,
            IHubContext<NotificationHub> hubContext,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _userNotificationRepository = userNotificationRepository;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<Notification> CreateNotification(CreateNotificationDto notificationDto)
        {
            try
            {
                _logger.LogInformation("Creating notification for entity: {EntityName}, entityId: {EntityId}", 
                    notificationDto.EntityName, notificationDto.EntityId);

                var notification = new Notification
                {
                    EntityName = notificationDto.EntityName,
                    EntityId = notificationDto.EntityId,
                    Content = notificationDto.Content,
                    CreatedAt = DateTime.UtcNow
                };

                var createdNotification = await _notificationRepository.Add(notification);

                // Create user notifications for each user
                foreach (var userId in notificationDto.UserIds)
                {
                    var userNotification = new UserNotifications
                    {
                        NotificationId = createdNotification.Id,
                        UserId = userId,
                        IsRead = false
                    };

                                    await _userNotificationRepository.Add(userNotification);

                // Send real-time notification to the user
                try
                {
                     var realTimeNotification = new NotificationDto
                    {
                        Id = createdNotification.Id,
                        EntityName = createdNotification.EntityName,
                        EntityId = createdNotification.EntityId,
                        Content = createdNotification.Content,
                        CreatedAt = createdNotification.CreatedAt,
                        IsRead = false
                    };

                    await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", realTimeNotification);
                    _logger.LogInformation("Real-time notification sent to user: {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send real-time notification to user: {UserId}", userId);
                }
            }

            _logger.LogInformation("Notification created successfully with ID: {Id} for {Count} users", 
                createdNotification.Id, notificationDto.UserIds.Count);

            return createdNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create notification");
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotifications(Guid userId, bool? isRead = null)
        {
            try
            {
                _logger.LogInformation("Fetching notifications for user: {UserId}, isRead filter: {IsRead}", userId, isRead);
                
                var userNotifications = await _userNotificationRepository.GetUserNotifications(userId, isRead);
                
                var notificationDtos = userNotifications.Select(un => new NotificationDto
                {
                    Id = un.Notification.Id,
                    EntityName = un.Notification.EntityName,
                    EntityId = un.Notification.EntityId,
                    Content = un.Notification.Content,
                    CreatedAt = un.Notification.CreatedAt,
                    IsRead = un.IsRead,
                    ReadAt = un.ReadAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} notifications for user: {UserId}", notificationDtos.Count, userId);
                return notificationDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notifications for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<NotificationDto> MarkNotificationAsRead(Guid userId, Guid notificationId)
        {
            try
            {
                _logger.LogInformation("Marking notification as read for user: {UserId}, notification: {NotificationId}", 
                    userId, notificationId);

                var userNotifications = await _userNotificationRepository.GetUserNotifications(userId);
                var userNotification = userNotifications.FirstOrDefault(un => un.NotificationId == notificationId);

                if (userNotification == null)
                {
                    _logger.LogWarning("User notification not found for user: {UserId}, notification: {NotificationId}", 
                        userId, notificationId);
                    throw new KeyNotFoundException("User notification not found");
                }

                userNotification.IsRead = true;
                userNotification.ReadAt = DateTime.UtcNow;

                await _userNotificationRepository.Update(userNotification.Id, userNotification);

                var notificationDto = new NotificationDto
                {
                    Id = userNotification.Notification.Id,
                    EntityName = userNotification.Notification.EntityName,
                    EntityId = userNotification.Notification.EntityId,
                    Content = userNotification.Notification.Content,
                    CreatedAt = userNotification.Notification.CreatedAt,
                    IsRead = userNotification.IsRead,
                    ReadAt = userNotification.ReadAt
                };

                // Send real-time update for unread count
                try
                {
                    var unreadCount = await GetUnreadNotificationCount(userId);
                    await _hubContext.Clients.Group(userId.ToString()).SendAsync("UpdateUnreadCount", unreadCount);
                    await _hubContext.Clients.Group(userId.ToString()).SendAsync("NotificationRead", notificationDto);
                    _logger.LogInformation("Real-time unread count update sent to user: {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send real-time unread count update to user: {UserId}", userId);
                }

                _logger.LogInformation("Notification marked as read for user: {UserId}, notification: {NotificationId}", 
                    userId, notificationId);

                return notificationDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notification as read for user: {UserId}, notification: {NotificationId}", 
                    userId, notificationId);
                throw;
            }
        }

        public async Task<bool> MarkAllNotificationsAsRead(Guid userId)
        {
            try
            {
                _logger.LogInformation("Marking all notifications as read for user: {UserId}", userId);
                var result = await _userNotificationRepository.MarkAllNotificationsAsRead(userId);
                
                // Send real-time update for unread count
                try
                {
                    var unreadCount = await GetUnreadNotificationCount(userId);
                    await _hubContext.Clients.Group(userId.ToString()).SendAsync("UpdateUnreadCount", unreadCount);
                    await _hubContext.Clients.Group(userId.ToString()).SendAsync("AllNotificationsRead");
                    _logger.LogInformation("Real-time all notifications read update sent to user: {UserId}", userId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send real-time all notifications read update to user: {UserId}", userId);
                }
                
                _logger.LogInformation("All notifications marked as read for user: {UserId}", userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all notifications as read for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetUnreadNotificationCount(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting unread notification count for user: {UserId}", userId);
                var count = await _userNotificationRepository.GetUnreadNotificationCount(userId);
                _logger.LogInformation("Unread notification count for user {UserId}: {Count}", userId, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread notification count for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteNotification(Guid notificationId)
        {
            try
            {
                _logger.LogInformation("Deleting notification with ID: {Id}", notificationId);
                await _notificationRepository.Delete(notificationId);
                _logger.LogInformation("Notification deleted successfully with ID: {Id}", notificationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete notification with ID: {Id}", notificationId);
                throw;
            }
        }
    }
} 