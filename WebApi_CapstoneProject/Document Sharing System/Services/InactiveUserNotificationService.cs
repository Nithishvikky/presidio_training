using DSS.Interfaces;
using DSS.Models.DTOs;

namespace DSS.Services
{
    public class InactiveUserNotificationService : IInactiveUserNotificationService
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<InactiveUserNotificationService> _logger;

        public InactiveUserNotificationService(
            IUserService userService,
            INotificationService notificationService,
            ILogger<InactiveUserNotificationService> logger)
        {
            _userService = userService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task NotifyInactiveUsersAsync()
        {
            try
            {
                _logger.LogInformation("Starting inactive user notification check");

                // Get users who have been inactive for 1 month (30 days)
                var inactivityThreshold = TimeSpan.FromDays(30);
                var inactiveUsers = await _userService.GetInactiveUsers(inactivityThreshold);

                if (!inactiveUsers.Any())
                {
                    _logger.LogInformation("No inactive users found");
                    return;
                }

                var userIds = inactiveUsers.Select(u => u.Id).ToList();

                // Create notification for inactive users
                var notificationDto = new CreateNotificationDto
                {
                    EntityName = "System",
                    EntityId = Guid.NewGuid(), // System notification
                    Content = "Your account has been inactive for 1 month. Your stored documents will be archived soon. Please log in to keep your account active.",
                    UserIds = userIds
                };

                await _notificationService.CreateNotification(notificationDto);

                _logger.LogInformation("Sent inactivity notifications to {Count} users", userIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while notifying inactive users");
                throw;
            }
        }

        // public async Task ArchiveAllFilesOfUser(Guid userId)
        // {
        //     await _userDocService.ArchiveAllFilesOfUser(userId);
        // }

    }
} 