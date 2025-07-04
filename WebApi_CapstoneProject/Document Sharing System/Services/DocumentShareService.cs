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
                                    IHubContext<NotificationHub> hub)
        {
            _shareRepo = shareRepo;
            _logger = logger;
            _userService = userService;
            _userDocService = userDocService;
            _userDocRepository = userDocRepository;
            _userRepository = userRepository;
            _documentViewRepository = documentViewRepository;
            _hub = hub;
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
            var share = new DocumentShare
            {
                DocumentId = document.Id,
                SharedWithUserId = user.Id
            };

            var sharedRespose = new SharedResponseeDto
            {
                FileName = document.FileName,
                Email = fileOwner.Email,
                UserName = fileOwner.Username,
                GrantedAt = DateTime.Now
            };

            await _hub.Clients.Group(SharedWithUserEmail).SendAsync("DocumentGiven",sharedRespose);

            return await _shareRepo.Add(share);
        }

        public async Task<ICollection<DocumentShare>> GrantAllPermission(string fileName, string UploaderEmail)
        {
            var document = await _userDocService.GetByFileName(fileName, UploaderEmail); // to check user has this file and also get file
            var users = await _userService.GetAllUsersOnly();

            var grantedShares = new List<DocumentShare>();

            foreach (var user in users)
            {
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

                    grantedShares.Add(await _shareRepo.Add(newShare));
                }
            }
            if (grantedShares == null || grantedShares.Count == 0)
            {
                throw new Exception("All users already granted");
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

            if (shares == null)
            {
                _logger.LogInformation("Permission not found or already revoked.{documentId}", document.Id);
                throw new ArgumentNullException("No users has been granted for this file");
            }
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

            if (sharedUsers == null || sharedUsers.Count == 0)
            {
                throw new ArgumentNullException("No Users have permission for this file");
            }
            return sharedUsers;
        }

        public async Task<ICollection<UserDocDetailDto>> GetFilesSharedWithUser(Guid userId)
        {
            var user = await _userService.GetUserById(userId);
            var shares = (await _shareRepo.GetAll()).Where(s => s.SharedWithUserId == userId && !s.IsRevoked);
            if (shares == null)
            {
                _logger.LogInformation("Permission not found or already revoked.{userId}", userId);
                throw new ArgumentNullException("No files has been granted for this user");
            }

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
            if (sharedFiles == null || sharedFiles.Count == 0)
            {
                throw new ArgumentNullException("User has no permission for any files");
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