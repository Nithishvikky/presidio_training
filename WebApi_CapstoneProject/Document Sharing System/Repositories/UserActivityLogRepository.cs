using DSS.Contexts;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class UserActivityLogRepository : IRepository<Guid, UserActivityLog>
    {
        private readonly DssContext _context;
        private readonly ILogger<UserActivityLogRepository> _logger;

        public UserActivityLogRepository(DssContext context, ILogger<UserActivityLogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserActivityLog> Add(UserActivityLog item)
        {
            try
            {
                _logger.LogInformation("Adding activity log for user: {UserId}, activity: {ActivityType}", 
                    item.UserId, item.ActivityType);
                var result = await _context.UserActivityLogs.AddAsync(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Activity log added successfully with ID: {Id}", result.Entity.Id);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add activity log");
                throw;
            }
        }

        public async Task<UserActivityLog> Delete(Guid key)
        {
            try
            {
                _logger.LogInformation("Deleting activity log with ID: {Id}", key);
                var activityLog = await _context.UserActivityLogs.FindAsync(key);
                if (activityLog == null)
                {
                    _logger.LogWarning("Activity log not found with ID: {Id}", key);
                    throw new KeyNotFoundException("Activity log not found");
                }

                _context.UserActivityLogs.Remove(activityLog);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Activity log deleted successfully with ID: {Id}", key);
                return activityLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete activity log with ID: {Id}", key);
                throw;
            }
        }

        public async Task<UserActivityLog> Get(Guid key)
        {
            try
            {
                _logger.LogInformation("Fetching activity log with ID: {Id}", key);
                var activityLog = await _context.UserActivityLogs
                    .Include(al => al.User)
                    .FirstOrDefaultAsync(al => al.Id == key);

                if (activityLog == null)
                {
                    _logger.LogWarning("Activity log not found with ID: {Id}", key);
                    throw new KeyNotFoundException("Activity log not found");
                }

                _logger.LogInformation("Activity log found with ID: {Id}", key);
                return activityLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch activity log with ID: {Id}", key);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLog>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all activity logs");
                var activityLogs = await _context.UserActivityLogs
                    .Include(al => al.User)
                    .OrderByDescending(al => al.Timestamp)
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} activity logs", activityLogs.Count);
                return activityLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all activity logs");
                throw;
            }
        }

        public async Task<UserActivityLog> Update(Guid key, UserActivityLog item)
        {
            try
            {
                _logger.LogInformation("Updating activity log with ID: {Id}", key);
                var existingActivityLog = await _context.UserActivityLogs.FindAsync(key);
                if (existingActivityLog == null)
                {
                    _logger.LogWarning("Activity log not found with ID: {Id}", key);
                    throw new KeyNotFoundException("Activity log not found");
                }

                // Activity logs are typically read-only, but if needed, update fields here
                existingActivityLog.Description = item.Description;

                _context.UserActivityLogs.Update(existingActivityLog);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Activity log updated successfully with ID: {Id}", key);
                return existingActivityLog;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update activity log with ID: {Id}", key);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLog>> GetUserActivityLogs(Guid userId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("Fetching activity logs for user: {UserId}, page: {PageNumber}, size: {PageSize}", 
                    userId, pageNumber, pageSize);
                
                var activityLogs = await _context.UserActivityLogs
                    .Include(al => al.User)
                    .Where(al => al.UserId == userId)
                    .OrderByDescending(al => al.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} activity logs for user: {UserId}", activityLogs.Count, userId);
                return activityLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch activity logs for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetUserActivityCount(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting activity count for user: {UserId}", userId);
                var count = await _context.UserActivityLogs
                    .CountAsync(al => al.UserId == userId);
                _logger.LogInformation("Activity count for user {UserId}: {Count}", userId, count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get activity count for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLog>> GetActivityLogsByType(string activityType, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching activity logs by type: {ActivityType}, page: {PageNumber}, size: {PageSize}", 
                    activityType, pageNumber, pageSize);
                
                var activityLogs = await _context.UserActivityLogs
                    .Include(al => al.User)
                    .Where(al => al.ActivityType == activityType)
                    .OrderByDescending(al => al.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} activity logs for type: {ActivityType}", activityLogs.Count, activityType);
                return activityLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch activity logs by type: {ActivityType}", activityType);
                throw;
            }
        }

        public async Task<IEnumerable<UserActivityLog>> GetActivityLogsByDateRange(DateTime startDate, DateTime endDate, int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching activity logs from {StartDate} to {EndDate}, page: {PageNumber}, size: {PageSize}", 
                    startDate, endDate, pageNumber, pageSize);
                
                var activityLogs = await _context.UserActivityLogs
                    .Include(al => al.User)
                    .Where(al => al.Timestamp >= startDate && al.Timestamp <= endDate)
                    .OrderByDescending(al => al.Timestamp)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} activity logs for date range", activityLogs.Count);
                return activityLogs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch activity logs by date range");
                throw;
            }
        }
    }
} 