using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateNotification(CreateNotificationDto notificationDto);
        Task<IEnumerable<NotificationDto>> GetUserNotifications(Guid userId, bool? isRead = null);
        Task<NotificationDto> MarkNotificationAsRead(Guid userId, Guid notificationId);
        Task<bool> MarkAllNotificationsAsRead(Guid userId);
        Task<int> GetUnreadNotificationCount(Guid userId);
        Task<bool> DeleteNotification(Guid notificationId);
    }
} 