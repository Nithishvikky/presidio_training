using DSS.Contexts;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class UserNotificationRepository : IRepository<Guid, UserNotifications>
    {
        private readonly DssContext _context;
        private readonly ILogger<UserNotificationRepository> _logger;

        public UserNotificationRepository(DssContext context, ILogger<UserNotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserNotifications> Add(UserNotifications item)
        {
            try
            {
                _logger.LogInformation("Adding new user notification for user: {UserId}, notification: {NotificationId}", 
                    item.UserId, item.NotificationId);
                var result = await _context.UserNotifications.AddAsync(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User notification added successfully with ID: {Id}", result.Entity.Id);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user notification");
                throw;
            }
        }

        public async Task<UserNotifications> Delete(Guid key)
        {
            try
            {
                _logger.LogInformation("Deleting user notification with ID: {Id}", key);
                var userNotification = await _context.UserNotifications.FindAsync(key);
                if (userNotification == null)
                {
                    _logger.LogWarning("User notification not found with ID: {Id}", key);
                    throw new KeyNotFoundException("User notification not found");
                }

                _context.UserNotifications.Remove(userNotification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User notification deleted successfully with ID: {Id}", key);
                return userNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user notification with ID: {Id}", key);
                throw;
            }
        }

        public async Task<UserNotifications> Get(Guid key)
        {
            try
            {
                _logger.LogInformation("Fetching user notification with ID: {Id}", key);
                var userNotification = await _context.UserNotifications
                    .Include(un => un.Notification)
                    .Include(un => un.User)
                    .FirstOrDefaultAsync(un => un.Id == key);

                if (userNotification == null)
                {
                    _logger.LogWarning("User notification not found with ID: {Id}", key);
                    throw new KeyNotFoundException("User notification not found");
                }

                _logger.LogInformation("User notification found with ID: {Id}", key);
                return userNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user notification with ID: {Id}", key);
                throw;
            }
        }

        public async Task<IEnumerable<UserNotifications>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all user notifications");
                var userNotifications = await _context.UserNotifications
                    .Include(un => un.Notification)
                    .Include(un => un.User)
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} user notifications", userNotifications.Count);
                return userNotifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all user notifications");
                throw;
            }
        }

        public async Task<UserNotifications> Update(Guid key, UserNotifications item)
        {
            try
            {
                _logger.LogInformation("Updating user notification with ID: {Id}", key);
                var existingUserNotification = await _context.UserNotifications.FindAsync(key);
                if (existingUserNotification == null)
                {
                    _logger.LogWarning("User notification not found with ID: {Id}", key);
                    throw new KeyNotFoundException("User notification not found");
                }

                existingUserNotification.IsRead = item.IsRead;
                existingUserNotification.ReadAt = item.ReadAt;

                _context.UserNotifications.Update(existingUserNotification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User notification updated successfully with ID: {Id}", key);
                return existingUserNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user notification with ID: {Id}", key);
                throw;
            }
        }

        public async Task<IEnumerable<UserNotifications>> GetUserNotifications(Guid userId, bool? isRead = null)
        {
            try
            {
                _logger.LogInformation("Fetching notifications for user: {UserId}, isRead filter: {IsRead}", userId, isRead);
                var query = _context.UserNotifications
                    .Include(un => un.Notification)
                    .Include(un => un.User)
                    .Where(un => un.UserId == userId);

                if (isRead.HasValue)
                {
                    query = query.Where(un => un.IsRead == isRead.Value);
                }

                var userNotifications = await query.OrderByDescending(un => un.Notification.CreatedAt).ToListAsync();
                _logger.LogInformation("Retrieved {Count} notifications for user: {UserId}", userNotifications.Count, userId);
                return userNotifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch notifications for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetUnreadNotificationCount(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting unread notification count for user: {UserId}", userId);
                var count = await _context.UserNotifications
                    .CountAsync(un => un.UserId == userId && !un.IsRead);
                _logger.LogInformation("Unread notification count for user {UserId}: {Count}", userId, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread notification count for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> MarkAllNotificationsAsRead(Guid userId)
        {
            try
            {
                _logger.LogInformation("Marking all notifications as read for user: {UserId}", userId);
                var unreadNotifications = await _context.UserNotifications
                    .Where(un => un.UserId == userId && !un.IsRead)
                    .ToListAsync();

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Marked {Count} notifications as read for user: {UserId}", unreadNotifications.Count, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all notifications as read for user: {UserId}", userId);
                throw;
            }
        }
    }
} 