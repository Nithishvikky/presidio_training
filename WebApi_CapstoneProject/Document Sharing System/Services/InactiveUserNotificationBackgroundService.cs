using DSS.Interfaces;

namespace DSS.Services
{
    public class InactiveUserNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InactiveUserNotificationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromDays(1); // Run daily

        public InactiveUserNotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<InactiveUserNotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Inactive User Notification Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Running daily inactive user check at {Time}", DateTime.UtcNow);

                    using var scope = _serviceProvider.CreateScope();
                    var inactiveUserNotificationService = scope.ServiceProvider.GetRequiredService<IInactiveUserNotificationService>();

                    await inactiveUserNotificationService.NotifyInactiveUsersAsync();

                    _logger.LogInformation("Daily inactive user check completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during daily inactive user check");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Inactive User Notification Background Service stopped");
        }
    }
} 