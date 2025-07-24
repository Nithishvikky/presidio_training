using DSS.Contexts;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class NotificationRepository : IRepository<Guid, Notification>
    {
        private readonly DssContext _context;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(DssContext context, ILogger<NotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Notification> Add(Notification item)
        {
            try
            {
                _logger.LogInformation("Adding new notification with content: {Content}", item.Content);
                var result = await _context.Notifications.AddAsync(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Notification added successfully with ID: {Id}", result.Entity.Id);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add notification");
                throw;
            }
        }

        public async Task<Notification> Delete(Guid key)
        {
            try
            {
                _logger.LogInformation("Deleting notification with ID: {Id}", key);
                var notification = await _context.Notifications.FindAsync(key);
                if (notification == null)
                {
                    _logger.LogWarning("Notification not found with ID: {Id}", key);
                    throw new KeyNotFoundException("Notification not found");
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Notification deleted successfully with ID: {Id}", key);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete notification with ID: {Id}", key);
                throw;
            }
        }

        public async Task<Notification> Get(Guid key)
        {
            try
            {
                _logger.LogInformation("Fetching notification with ID: {Id}", key);
                var notification = await _context.Notifications
                    .Include(n => n.NotificationUsers)
                    .ThenInclude(un => un.User)
                    .FirstOrDefaultAsync(n => n.Id == key);

                if (notification == null)
                {
                    _logger.LogWarning("Notification not found with ID: {Id}", key);
                    throw new KeyNotFoundException("Notification not found");
                }

                _logger.LogInformation("Notification found with ID: {Id}", key);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch notification with ID: {Id}", key);
                throw;
            }
        }

        public async Task<IEnumerable<Notification>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all notifications");
                var notifications = await _context.Notifications
                    .Include(n => n.NotificationUsers)
                    .ThenInclude(un => un.User)
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} notifications", notifications.Count);
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all notifications");
                throw;
            }
        }

        public async Task<Notification> Update(Guid key, Notification item)
        {
            try
            {
                _logger.LogInformation("Updating notification with ID: {Id}", key);
                var existingNotification = await _context.Notifications.FindAsync(key);
                if (existingNotification == null)
                {
                    _logger.LogWarning("Notification not found with ID: {Id}", key);
                    throw new KeyNotFoundException("Notification not found");
                }

                existingNotification.EntityName = item.EntityName;
                existingNotification.EntityId = item.EntityId;
                existingNotification.Content = item.Content;

                _context.Notifications.Update(existingNotification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Notification updated successfully with ID: {Id}", key);
                return existingNotification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update notification with ID: {Id}", key);
                throw;
            }
        }
    }
} 