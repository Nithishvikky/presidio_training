using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using DSS.Repositories;

namespace DSS.Services
{
    public class UserRequestService : IUserRequestService
    {
        private readonly IRepository<Guid, UserRequest> _userRequestRepository;
        private readonly UserRequestRepository _userRequestRepositoryExtended;
        private readonly IRepository<Guid, UserDocument> _userDocumentRepository;
        private readonly IUserService _userService;
        private readonly ILogger<UserRequestService> _logger;

        public UserRequestService(
            IRepository<Guid, UserRequest> userRequestRepository,
            UserRequestRepository userRequestRepositoryExtended,
            IRepository<Guid, UserDocument> userDocumentRepository,
            IUserService userService,
            ILogger<UserRequestService> logger)
        {
            _userRequestRepository = userRequestRepository;
            _userRequestRepositoryExtended = userRequestRepositoryExtended;
            _userDocumentRepository = userDocumentRepository;
            _userService = userService;
            _logger = logger;
        }

        public async Task<UserRequestDto> CreateRequestAsync(CreateUserRequestDto requestDto, Guid userId)
        {
            try
            {
                _logger.LogInformation("Creating request for user: {UserId}, document: {DocumentId}", 
                    userId, requestDto.DocumentId);

                // Check if document exists and is archived
                var document = await _userDocumentRepository.Get(requestDto.DocumentId);
                if (document.Status != "Archived")
                {
                    throw new InvalidOperationException("Document is not archived. Access requests are only for archived documents.");
                }

                // Check if user already has a pending request for this document
                var existingRequests = await _userRequestRepositoryExtended.GetUserRequests(userId);
                var pendingRequest = existingRequests.FirstOrDefault(ur => 
                    ur.DocumentId == requestDto.DocumentId && ur.Status == "Pending");
                
                if (pendingRequest != null)
                {
                    throw new InvalidOperationException("You already have a pending request for this document.");
                }

                var userRequest = new UserRequest
                {
                    UserId = userId,
                    DocumentId = requestDto.DocumentId,
                    RequestType = requestDto.RequestType,
                    Reason = requestDto.Reason,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow,
                    AccessDurationHours = requestDto.AccessDurationHours ?? 24
                };

                var result = await _userRequestRepository.Add(userRequest);

                var requestDtoResult = new UserRequestDto
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    UserEmail = string.Empty, // Will be populated when fetched with includes
                    UserUsername = string.Empty, // Will be populated when fetched with includes
                    DocumentId = result.DocumentId,
                    DocumentFileName = document.FileName,
                    RequestType = result.RequestType,
                    Status = result.Status,
                    Reason = result.Reason,
                    RequestedAt = result.RequestedAt,
                    AccessDurationHours = result.AccessDurationHours
                };

                _logger.LogInformation("Request created successfully with ID: {Id}", result.Id);
                return requestDtoResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create request for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserRequestDto> ProcessRequestAsync(ProcessUserRequestDto processDto, Guid adminUserId)
        {
            try
            {
                _logger.LogInformation("Processing request: {RequestId} by admin: {AdminUserId}", 
                    processDto.RequestId, adminUserId);

                var userRequest = await _userRequestRepository.Get(processDto.RequestId);
                if (userRequest.Status != "Pending")
                {
                    throw new InvalidOperationException("Request has already been processed.");
                }

                userRequest.Status = processDto.Status;
                userRequest.ProcessedAt = DateTime.UtcNow;

                if (processDto.Status == "Approved")
                {
                    // Temporarily unarchive the document
                    var document = await _userDocumentRepository.Get(userRequest.DocumentId);
                    document.Status = "TemporarilyUnarchived";
                    document.TemporarilyUnarchivedAt = DateTime.UtcNow;
                    document.ScheduledRearchiveAt = DateTime.UtcNow.AddHours(processDto.AccessDurationHours ?? 24);
                    
                    await _userDocumentRepository.Update(document.Id, document);

                    userRequest.AccessGrantedAt = DateTime.UtcNow;
                    userRequest.AccessExpiresAt = document.ScheduledRearchiveAt;
                    userRequest.AccessDurationHours = processDto.AccessDurationHours ?? 24;

                    _logger.LogInformation("Document {DocumentId} temporarily unarchived until {ExpiresAt}", 
                        document.Id, document.ScheduledRearchiveAt);
                }

                var updatedRequest = await _userRequestRepository.Update(userRequest.Id, userRequest);

                var requestDtoResult = new UserRequestDto
                {
                    Id = updatedRequest.Id,
                    UserId = updatedRequest.UserId,
                    UserEmail = updatedRequest.User?.Email ?? string.Empty,
                    UserUsername = updatedRequest.User?.Username ?? string.Empty,
                    DocumentId = updatedRequest.DocumentId,
                    DocumentFileName = updatedRequest.Document?.FileName ?? string.Empty,
                    RequestType = updatedRequest.RequestType,
                    Status = updatedRequest.Status,
                    Reason = updatedRequest.Reason,
                    RequestedAt = updatedRequest.RequestedAt,
                    ProcessedAt = updatedRequest.ProcessedAt,
                    AccessGrantedAt = updatedRequest.AccessGrantedAt,
                    AccessExpiresAt = updatedRequest.AccessExpiresAt,
                    AccessDurationHours = updatedRequest.AccessDurationHours
                };

                _logger.LogInformation("Request {RequestId} processed successfully with status: {Status}", 
                    processDto.RequestId, processDto.Status);

                return requestDtoResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process request: {RequestId}", processDto.RequestId);
                throw;
            }
        }

        public async Task<IEnumerable<UserRequestDto>> GetUserRequestsAsync(Guid userId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("Fetching requests for user: {UserId}, page: {PageNumber}, size: {PageSize}", 
                    userId, pageNumber, pageSize);

                var userRequests = await _userRequestRepositoryExtended.GetUserRequests(userId, pageNumber, pageSize);

                var requestDtos = userRequests.Select(ur => new UserRequestDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserEmail = ur.User?.Email ?? string.Empty,
                    UserUsername = ur.User?.Username ?? string.Empty,
                    DocumentId = ur.DocumentId,
                    DocumentFileName = ur.Document?.FileName ?? string.Empty,
                    RequestType = ur.RequestType,
                    Status = ur.Status,
                    Reason = ur.Reason,
                    RequestedAt = ur.RequestedAt,
                    ProcessedAt = ur.ProcessedAt,
                    AccessGrantedAt = ur.AccessGrantedAt,
                    AccessExpiresAt = ur.AccessExpiresAt,
                    AccessDurationHours = ur.AccessDurationHours
                }).ToList();

                _logger.LogInformation("Retrieved {Count} requests for user: {UserId}", requestDtos.Count, userId);
                return requestDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get requests for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserRequestDto>> GetAllRequestsAsync(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching all requests, page: {PageNumber}, size: {PageSize}", pageNumber, pageSize);

                var allRequests = await _userRequestRepository.GetAll();
                var pagedRequests = allRequests
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                var requestDtos = pagedRequests.Select(ur => new UserRequestDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserEmail = ur.User?.Email ?? string.Empty,
                    UserUsername = ur.User?.Username ?? string.Empty,
                    DocumentId = ur.DocumentId,
                    DocumentFileName = ur.Document?.FileName ?? string.Empty,
                    RequestType = ur.RequestType,
                    Status = ur.Status,
                    Reason = ur.Reason,
                    RequestedAt = ur.RequestedAt,
                    ProcessedAt = ur.ProcessedAt,
                    AccessGrantedAt = ur.AccessGrantedAt,
                    AccessExpiresAt = ur.AccessExpiresAt,
                    AccessDurationHours = ur.AccessDurationHours
                }).ToList();

                _logger.LogInformation("Retrieved {Count} requests", requestDtos.Count);
                return requestDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all requests");
                throw;
            }
        }

        public async Task<IEnumerable<UserRequestDto>> GetPendingRequestsAsync(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching pending requests, page: {PageNumber}, size: {PageSize}", pageNumber, pageSize);

                var pendingRequests = await _userRequestRepositoryExtended.GetPendingRequests(pageNumber, pageSize);

                var requestDtos = pendingRequests.Select(ur => new UserRequestDto
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserEmail = ur.User?.Email ?? string.Empty,
                    UserUsername = ur.User?.Username ?? string.Empty,
                    DocumentId = ur.DocumentId,
                    DocumentFileName = ur.Document?.FileName ?? string.Empty,
                    RequestType = ur.RequestType,
                    Status = ur.Status,
                    Reason = ur.Reason,
                    RequestedAt = ur.RequestedAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} pending requests", requestDtos.Count);
                return requestDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending requests");
                throw;
            }
        }

        public async Task<UserRequestDto> GetRequestByIdAsync(Guid requestId)
        {
            try
            {
                _logger.LogInformation("Fetching request by ID: {RequestId}", requestId);

                var userRequest = await _userRequestRepository.Get(requestId);

                var requestDto = new UserRequestDto
                {
                    Id = userRequest.Id,
                    UserId = userRequest.UserId,
                    UserEmail = userRequest.User?.Email ?? string.Empty,
                    UserUsername = userRequest.User?.Username ?? string.Empty,
                    DocumentId = userRequest.DocumentId,
                    DocumentFileName = userRequest.Document?.FileName ?? string.Empty,
                    RequestType = userRequest.RequestType,
                    Status = userRequest.Status,
                    Reason = userRequest.Reason,
                    RequestedAt = userRequest.RequestedAt,
                    ProcessedAt = userRequest.ProcessedAt,
                    AccessGrantedAt = userRequest.AccessGrantedAt,
                    AccessExpiresAt = userRequest.AccessExpiresAt,
                    AccessDurationHours = userRequest.AccessDurationHours
                };

                _logger.LogInformation("Request {RequestId} retrieved successfully", requestId);
                return requestDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get request by ID: {RequestId}", requestId);
                throw;
            }
        }

        public async Task<DocumentAccessStatusDto> GetDocumentAccessStatusAsync(Guid documentId, Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting document access status for document: {DocumentId}, user: {UserId}", 
                    documentId, userId);

                var document = await _userDocumentRepository.Get(documentId);
                var activeRequests = await _userRequestRepositoryExtended.GetActiveAccessRequests(userId);
                var activeRequest = activeRequests.FirstOrDefault(ur => ur.DocumentId == documentId);

                var accessStatus = new DocumentAccessStatusDto
                {
                    DocumentId = documentId,
                    DocumentFileName = document.FileName,
                    Status = document.Status,
                    HasAccess = activeRequest != null,
                    IsExpired = activeRequest?.AccessExpiresAt < DateTime.UtcNow,
                    AccessExpiresAt = activeRequest?.AccessExpiresAt
                };

                _logger.LogInformation("Document access status retrieved for document: {DocumentId}", documentId);
                return accessStatus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get document access status for document: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentAccessStatusDto>> GetUserAccessibleDocumentsAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting accessible documents for user: {UserId}", userId);

                var activeRequests = await _userRequestRepositoryExtended.GetActiveAccessRequests(userId);

                var accessibleDocuments = activeRequests.Select(ur => new DocumentAccessStatusDto
                {
                    DocumentId = ur.DocumentId,
                    DocumentFileName = ur.Document?.FileName ?? string.Empty,
                    Status = ur.Document?.Status ?? "Unknown",
                    HasAccess = true,
                    IsExpired = ur.AccessExpiresAt < DateTime.UtcNow,
                    AccessExpiresAt = ur.AccessExpiresAt
                }).ToList();

                _logger.LogInformation("Retrieved {Count} accessible documents for user: {UserId}", 
                    accessibleDocuments.Count, userId);
                return accessibleDocuments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get accessible documents for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> HasDocumentAccessAsync(Guid documentId, Guid userId)
        {
            try
            {
                _logger.LogInformation("Checking document access for document: {DocumentId}, user: {UserId}", 
                    documentId, userId);

                var activeRequests = await _userRequestRepositoryExtended.GetActiveAccessRequests(userId);
                var hasAccess = activeRequests.Any(ur => ur.DocumentId == documentId && ur.AccessExpiresAt > DateTime.UtcNow);

                _logger.LogInformation("User {UserId} has access to document {DocumentId}: {HasAccess}", 
                    userId, documentId, hasAccess);
                return hasAccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check document access for document: {DocumentId}", documentId);
                throw;
            }
        }
    }
} 