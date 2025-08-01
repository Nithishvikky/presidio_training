using DSS.Contexts;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class UserRequestRepository : IRepository<Guid, UserRequest>
    {
        private readonly DssContext _context;
        private readonly ILogger<UserRequestRepository> _logger;

        public UserRequestRepository(DssContext context, ILogger<UserRequestRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserRequest> Add(UserRequest item)
        {
            try
            {
                _logger.LogInformation("Adding user request for user: {UserId}, document: {DocumentId}", 
                    item.UserId, item.DocumentId);
                
                // Validate the entity before adding
                if (item.UserId == Guid.Empty)
                {
                    throw new ArgumentException("UserId cannot be empty");
                }
                if (item.DocumentId == Guid.Empty)
                {
                    throw new ArgumentException("DocumentId cannot be empty");
                }
                
                var result = await _context.UserRequests.AddAsync(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User request added successfully with ID: {Id}", result.Entity.Id);
                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user request for user: {UserId}, document: {DocumentId}", 
                    item.UserId, item.DocumentId);
                throw;
            }
        }

        public async Task<UserRequest> Delete(Guid key)
        {
            try
            {
                _logger.LogInformation("Deleting user request with ID: {Id}", key);
                var userRequest = await _context.UserRequests.FindAsync(key);
                if (userRequest == null)
                {
                    _logger.LogWarning("User request not found with ID: {Id}", key);
                    throw new KeyNotFoundException("User request not found");
                }

                _context.UserRequests.Remove(userRequest);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User request deleted successfully with ID: {Id}", key);
                return userRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user request with ID: {Id}", key);
                throw;
            }
        }

        public async Task<UserRequest> Get(Guid key)
        {
            try
            {
                _logger.LogInformation("Fetching user request with ID: {Id}", key);
                var userRequest = await _context.UserRequests
                    .Include(ur => ur.User)
                    .Include(ur => ur.Document)
                    .FirstOrDefaultAsync(ur => ur.Id == key);

                if (userRequest == null)
                {
                    _logger.LogWarning("User request not found with ID: {Id}", key);
                    throw new KeyNotFoundException("User request not found");
                }

                _logger.LogInformation("User request found with ID: {Id}", key);
                return userRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user request with ID: {Id}", key);
                throw;
            }
        }

        public async Task<IEnumerable<UserRequest>> GetAll()
        {
            try
            {
                _logger.LogInformation("Fetching all user requests");
                var userRequests = await _context.UserRequests
                    .Include(ur => ur.User)
                    .Include(ur => ur.Document)
                    .OrderByDescending(ur => ur.RequestedAt)
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} user requests", userRequests.Count);
                return userRequests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch all user requests");
                throw;
            }
        }

        public async Task<UserRequest> Update(Guid key, UserRequest item)
        {
            try
            {
                _logger.LogInformation("Updating user request with ID: {Id}", key);
                var existingUserRequest = await _context.UserRequests.FindAsync(key);
                if (existingUserRequest == null)
                {
                    _logger.LogWarning("User request not found with ID: {Id}", key);
                    throw new KeyNotFoundException("User request not found");
                }

                existingUserRequest.Status = item.Status;
                existingUserRequest.ProcessedAt = item.ProcessedAt;
                existingUserRequest.AccessGrantedAt = item.AccessGrantedAt;
                existingUserRequest.AccessExpiresAt = item.AccessExpiresAt;
                existingUserRequest.AccessDurationHours = item.AccessDurationHours;

                _context.UserRequests.Update(existingUserRequest);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User request updated successfully with ID: {Id}", key);
                return existingUserRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user request with ID: {Id}", key);
                throw;
            }
        }

        public async Task<IEnumerable<UserRequest>> GetUserRequests(Guid userId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("Fetching user requests for user: {UserId}, page: {PageNumber}, size: {PageSize}", 
                    userId, pageNumber, pageSize);
                
                var userRequests = await _context.UserRequests
                    .Include(ur => ur.User)
                    .Include(ur => ur.Document)
                    .Where(ur => ur.UserId == userId)
                    .OrderByDescending(ur => ur.RequestedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} user requests for user: {UserId}", userRequests.Count, userId);
                return userRequests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch user requests for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserRequest>> GetPendingRequests(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching pending requests, page: {PageNumber}, size: {PageSize}", pageNumber, pageSize);
                
                var pendingRequests = await _context.UserRequests
                    .Include(ur => ur.User)
                    .Include(ur => ur.Document)
                    .Where(ur => ur.Status == "Pending")
                    .OrderByDescending(ur => ur.RequestedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} pending requests", pendingRequests.Count);
                return pendingRequests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch pending requests");
                throw;
            }
        }

        public async Task<IEnumerable<UserRequest>> GetActiveAccessRequests(Guid userId)
        {
            try
            {
                _logger.LogInformation("Fetching active access requests for user: {UserId}", userId);
                
                var activeRequests = await _context.UserRequests
                    .Include(ur => ur.Document)
                    .Where(ur => ur.UserId == userId && 
                                ur.Status == "Approved" && 
                                ur.AccessExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(ur => ur.AccessExpiresAt)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} active access requests for user: {UserId}", activeRequests.Count, userId);
                return activeRequests;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch active access requests for user: {UserId}", userId);
                throw;
            }
        }
    }
} 