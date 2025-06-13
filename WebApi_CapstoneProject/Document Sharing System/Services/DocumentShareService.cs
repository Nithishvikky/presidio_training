using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Services
{
    public class DocumentShareService : IDocumentShareService
    {
        private readonly IRepository<Guid, DocumentShare> _shareRepo;
        private readonly ILogger<DocumentShareService> _logger;
        private readonly IUserService _userService;
        private readonly IUserDocService _userDocService;
        private IRepository<Guid, UserDocument> _userDocRepository;

        public DocumentShareService(IRepository<Guid, DocumentShare> shareRepo,
                                    ILogger<DocumentShareService> logger,
                                    IUserService userService,
                                    IUserDocService userDocService,
                                    IRepository<Guid, UserDocument> userDocRepository)
        {
            _shareRepo = shareRepo;
            _logger = logger;
            _userService = userService;
            _userDocService = userDocService;
            _userDocRepository = userDocRepository;
        }

        public async Task<DocumentShare> GrantPermission(string fileName,string UploaderEmail, string SharedWithUserEmail)
        {
            var document = await _userDocService.GetByFileName(fileName,UploaderEmail); // to check user has this file and also get file
            var user = await _userService.GetUserByEmail(SharedWithUserEmail);

            var share = new DocumentShare
            {
                DocumentId = document.Id,
                SharedWithUserId = user.Id
            };

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
                throw new ArgumentNullException("No users granted");
            }
            return grantedShares;
        }
        
        public async Task<DocumentShare> RevokePermission(string fileName,string UploaderEmail, string SharedWithUserEmail)
        {
            var document = await _userDocService.GetByFileName(fileName,UploaderEmail); // to check user has this file and also get file
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

            var shares = (await _shareRepo.GetAll()).Where(s => s.DocumentId == document.Id);

            if (shares == null)
            {
                throw new ArgumentNullException("No users has been granted for this file");
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
                    UserName = user.Username
                });
            }

            if (sharedUsers == null || sharedUsers.Count == 0)
            {
                throw new ArgumentNullException("No Users have permission for this file");
            }
            return sharedUsers;
        }

        public async Task<ICollection<SharedResponseeDto>> GetFilesSharedWithUser(Guid userId)
        {
            var user = await _userService.GetUserById(userId);
            var shares = (await _shareRepo.GetAll()).Where(s => s.SharedWithUserId == userId);
            if (shares == null)
            {
                _logger.LogInformation("Permission not found or already revoked.{userId}", userId);
                throw new ArgumentNullException("No files has been granted for this user");
            }

            var sharedFiles = new List<SharedResponseeDto>();
            foreach (var share in shares)
            {
                var document = await _userDocRepository.Get(share.DocumentId);
                sharedFiles.Add(new SharedResponseeDto
                {
                    FileName = document.FileName,
                    UserName = user.Username
                });
            }
            if (sharedFiles == null || sharedFiles.Count == 0)
            {
                throw new ArgumentNullException("User has no permission for any files");
            }
            return sharedFiles;
        }
    }
}