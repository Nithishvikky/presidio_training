using System.Reflection.Metadata.Ecma335;
using DSS.Interfaces;
using DSS.Misc;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace DSS.Services
{
    public class DocumentShareService : IDocumentShareService
    {
        private readonly IRepository<Guid, DocumentShare> _shareRepo;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly ILogger<DocumentShareService> _logger;
        private readonly IUserService _userService;
        private readonly IUserDocService _userDocService;
        private readonly INotificationService _notificationService;
        private readonly IUserActivityLogService _userActivityLogService;

        private readonly IRepository<Guid, DocumentView> _documentViewRepository;
        private IRepository<Guid, UserDocument> _userDocRepository;
        private IRepository<Guid, User> _userRepository;

        public DocumentShareService(IRepository<Guid, DocumentShare> shareRepo,
                                    ILogger<DocumentShareService> logger,
                                    IUserService userService,
                                    IUserDocService userDocService,
                                    IRepository<Guid, UserDocument> userDocRepository,
                                    IRepository<Guid, User> userRepository,
                                    IRepository<Guid, DocumentView> documentViewRepository,
                                    IHubContext<NotificationHub> hub,
                                    INotificationService notificationService,
                                    IUserActivityLogService userActivityLogService)
        {
            _shareRepo = shareRepo;
            _logger = logger;
            _userService = userService;
            _userDocService = userDocService;
            _userDocRepository = userDocRepository;
            _userRepository = userRepository;
            _documentViewRepository = documentViewRepository;
            _hub = hub;
            _notificationService = notificationService;
            _userActivityLogService = userActivityLogService;
        }

        public async Task<DocumentShare> GrantPermission(string fileName, string UploaderEmail, string SharedWithUserEmail)
        {
            var document = await _userDocService.GetByFileName(fileName, UploaderEmail); // to check user has this file and also get file
            var fileOwner = await _userService.GetUserByEmail(UploaderEmail);
            var user = await _userService.GetUserByEmail(SharedWithUserEmail);

            var existingShares = await _shareRepo.GetAll();
            var existing = existingShares.SingleOrDefault(s =>
                    s.DocumentId == document.Id &&
                    s.SharedWithUserId == user.Id &&
                    !s.IsRevoked);

            if (existing != null)
            {
                throw new Exception("Already gave permission");
            }
            
            if (fileOwner.Id == user.Id)
            {
                throw new Exception("It's your document");
            }

            var share = new DocumentShare
            {
                DocumentId = document.Id,
                SharedWithUserId = user.Id
            };

            var createdShare = await _shareRepo.Add(share);

            // Create notification for the user who received the document
            try
            {
                var notificationDto = new CreateNotificationDto
                {
                    EntityName = "DocumentShare",
                    EntityId = createdShare.Id,
                    Content = $"{fileOwner.Username} has shared the document '{document.FileName}' with you.",
                    UserIds = new List<Guid> { user.Id }
                };

                await _notificationService.CreateNotification(notificationDto);
                _logger.LogInformation("Notification created for document share. ShareId: {ShareId}, UserId: {UserId}", 
                    createdShare.Id, user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create notification for document share. ShareId: {ShareId}, UserId: {UserId}", 
                    createdShare.Id, user.Id);
            }

            // Log user activity for document sharing
            try
            {
                var activityDto = new CreateActivityLogDto
                {
                    UserId = fileOwner.Id,
                    ActivityType = "DocumentShare",
                    Description = $"Shared document '{document.FileName}' with {user.Username} ({user.Email})"
                };
                await _userActivityLogService.LogActivityAsync(activityDto);
                _logger.LogInformation("Activity logged for document share. SharerId: {SharerId}, RecipientId: {RecipientId}, FileName: {FileName}", 
                    fileOwner.Id, user.Id, document.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log activity for document share. SharerId: {SharerId}, RecipientId: {RecipientId}, FileName: {FileName}", 
                    fileOwner.Id, user.Id, document.FileName);
            }

            var sharedRespose = new SharedResponseeDto
            {
                FileName = document.FileName,
                Email = fileOwner.Email,
                UserName = fileOwner.Username,
                GrantedAt = DateTime.Now
            };

            await _hub.Clients.Group(SharedWithUserEmail).SendAsync("DocumentGiven", sharedRespose);

            return createdShare;
        }

        public async Task<ICollection<DocumentShare>> GrantAllPermission(string fileName, string UploaderEmail)
        {
            var document = await _userDocService.GetByFileName(fileName, UploaderEmail); // to check user has this file and also get file
            var fileOwner = await _userService.GetUserByEmail(UploaderEmail);
            var users = await _userService.GetAllUsersOnly();

            var grantedShares = new List<DocumentShare>();

            foreach (var user in users)
            {
                if (user.Email == UploaderEmail)
                {
                    continue;
                }
                
                var existingShares = await _shareRepo.GetAll();
                var existing = existingShares.SingleOrDefault(s =>
                    s.DocumentId == document.Id &&
                    s.SharedWithUserId == user.Id &&
                    !s.IsRevoked);

                if (existing == null)
                {
                    var newShare = new DocumentShare
                    {
                        DocumentId = document.Id,
                        SharedWithUserId = user.Id,
                    };

                    var createdShare = await _shareRepo.Add(newShare);
                    grantedShares.Add(createdShare);

                    // Create notification for the user who received the document
                    try
                    {
                        var notificationDto = new CreateNotificationDto
                        {
                            EntityName = "DocumentShare",
                            EntityId = createdShare.Id,
                            Content = $"{fileOwner.Username} has shared the document '{document.FileName}' with you.",
                            UserIds = new List<Guid> { user.Id }
                        };

                        await _notificationService.CreateNotification(notificationDto);
                        _logger.LogInformation("Notification created for document share (all users). ShareId: {ShareId}, UserId: {UserId}", 
                            createdShare.Id, user.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to create notification for document share (all users). ShareId: {ShareId}, UserId: {UserId}", 
                            createdShare.Id, user.Id);
                    }
                }
            }
            if (grantedShares == null || grantedShares.Count == 0)
            {
                throw new Exception("All users already granted");
            }

            // Log user activity for sharing with all users
            try
            {
                var activityDto = new CreateActivityLogDto
                {
                    UserId = fileOwner.Id,
                    ActivityType = "DocumentShare",
                    Description = $"Shared document '{document.FileName}' with all users ({grantedShares.Count} users)"
                };
                await _userActivityLogService.LogActivityAsync(activityDto);
                _logger.LogInformation("Activity logged for document share (all users). SharerId: {SharerId}, FileName: {FileName}, Count: {Count}", 
                    fileOwner.Id, document.FileName, grantedShares.Count);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to log activity for document share (all users). SharerId: {SharerId}, FileName: {FileName}", 
                    fileOwner.Id, document.FileName);
            }

            return grantedShares;
        }

        public async Task<DocumentShare> RevokePermission(string fileName, string UploaderEmail, string SharedWithUserEmail)
        {
            var document = await _userDocService.GetByFileName(fileName, UploaderEmail); // to check user has this file and also get file
            var user = await _userService.GetUserByEmail(SharedWithUserEmail);

            var shares = await _shareRepo.GetAll();
            var share = shares.FirstOrDefault(s => s.DocumentId == document.Id && s.SharedWithUserId == user.Id && !s.IsRevoked);

            if (share == null)
            {
                _logger.LogInformation("Permission not found or already revoked.{documentId}", document.Id);
                throw new Exception("Permission not found or already revoked.");
            }

            share.IsRevoked = true;
            return await _shareRepo.Update(share.Id, share);
        }

        public async Task RevokeAllPermission(string fileName, string UploaderEmail)
        {
            var document = await _userDocService.GetByFileName(fileName, UploaderEmail); // to check user has this file and also get file

            var shares = (await _shareRepo.GetAll()).Where(s => s.DocumentId == document.Id && !s.IsRevoked).ToList();

            if (shares == null || shares.Count == 0)
            {
                throw new Exception("No users has been granted for this file");
            }

            foreach (var share in shares)
            {
                if (!share.IsRevoked)
                {
                    share.IsRevoked = true;
                    await _shareRepo.Update(share.Id, share);
                }
            }
        }
        public async Task<bool> IsDocumentSharedWithUser(Guid documentId, Guid SharedWithUserId)
        {

            var allShares = await _shareRepo.GetAll();
            var share = allShares.FirstOrDefault(s => s.DocumentId == documentId && s.SharedWithUserId == SharedWithUserId && !s.IsRevoked);

            if (share == null)
            {
                return false;
            }
            return true;
        }

        public async Task<ICollection<SharedResponseeDto>> GetSharedUsersByFileName(string fileName, string UploaderEmail)
        {
            var document = await _userDocService.GetByFileName(fileName, UploaderEmail); // to check user has this file and also get file
            var shares = (await _shareRepo.GetAll()).Where(s => s.DocumentId == document.Id && !s.IsRevoked);
            
            var sharedUsers = new List<SharedResponseeDto>();

            foreach (var share in shares)
            {
                var user = await _userService.GetUserById(share.SharedWithUserId);
                sharedUsers.Add(new SharedResponseeDto
                {
                    FileName = document.FileName,
                    Email = user.Email,
                    UserName = user.Username
                });
            }

            return sharedUsers;
        }

        public async Task<ICollection<UserDocDetailDto>> GetFilesSharedWithUser(Guid userId)
        {
            var user = await _userService.GetUserById(userId);
            var shares = (await _shareRepo.GetAll()).Where(s => s.SharedWithUserId == userId && !s.IsRevoked);

            var sharedFiles = new List<UserDocDetailDto>();
            foreach (var share in shares)
            {
                var document = await _userDocRepository.Get(share.DocumentId);
                if (!document.IsDeleted)
                {
                    var mapper = new UserDocMapper();
                    var documentDetails = mapper.MapUserDoc(document);
                    documentDetails.UploaderEmail = document.UploadedByUser.Email;
                    documentDetails.UploaderUsername = document.UploadedByUser.Username;
                    sharedFiles.Add(documentDetails);
                }
            }
            
            sharedFiles = sharedFiles.OrderByDescending(d => d.UploadedAt).ToList();
            return sharedFiles;
        }

        public async Task<DashboardDto> GetDashboard()
        {
            var totalUsers = (await _userRepository.GetAll()).ToList().Count;
            var totalAdmin = (await _userRepository.GetAll()).Where(u => u.Role == "Admin").ToList().Count;
            var totalusers = (await _userRepository.GetAll()).Where(u => u.Role == "User").ToList().Count;

            var allDocs = await _userDocRepository.GetAll();
            var totalDocs = allDocs.Where(d => !d.IsDeleted).ToList().Count;

            var docDict = allDocs.Where(d => !d.IsDeleted).ToDictionary(d => d.Id);

            var shares = await _shareRepo.GetAll();
            var totalShares = shares.Where(s => !s.IsRevoked && docDict.ContainsKey(s.DocumentId)).Count();

            var totalViews = (await _documentViewRepository.GetAll()).ToList().Count;

            var data = new DashboardDto
            {
                TotalUsers = totalUsers,
                TotalAdmin = totalAdmin,
                TotalUserRole = totalusers,
                TotalDocuments = totalDocs,
                TotalShared = totalShares,
                TotalViews = totalViews
            };

            return data;
        }

    }
}