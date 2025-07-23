using DSS.Interfaces;

namespace DSS.Services
{
    public class DocumentRearchiveBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DocumentRearchiveBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); // Run every hour

        public DocumentRearchiveBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<DocumentRearchiveBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Document Rearchive Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Running hourly document rearchive check at {Time}", DateTime.UtcNow);

                    using var scope = _serviceProvider.CreateScope();
                    var documentRearchiveService = scope.ServiceProvider.GetRequiredService<IDocumentRearchiveService>();

                    await documentRearchiveService.RearchiveExpiredDocumentsAsync();

                    _logger.LogInformation("Hourly document rearchive check completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during hourly document rearchive check");
                }

                // Wait for the next check interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Document Rearchive Background Service stopped");
        }
    }
} 