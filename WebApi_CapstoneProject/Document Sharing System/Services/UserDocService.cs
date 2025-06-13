using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Models;

namespace DSS.Services
{
    public class UserDocService : IUserDocService
    {
        private readonly IRepository<Guid, UserDocument> _userDocRepository;
        private readonly IUserService _userService;
        private readonly ILogger<UserDocService> _logger;

        public UserDocService(IRepository<Guid, UserDocument> userDocRepository,
                              IUserService userService,
                              ILogger<UserDocService> logger)
        {
            _userDocRepository = userDocRepository;
            _userService       = userService;
            _logger            = logger;
        }

        public async Task<UserDocument> UploadDoc(UserDocument doc)
        {
           try
            {
                var documents = await _userDocRepository.GetAll();
                if (documents.Any(d => d.FileName.Equals(doc.FileName,StringComparison.OrdinalIgnoreCase) && d.UploadedById==doc.UploadedById && !d.IsDeleted))
                {
                    _logger.LogWarning("You already uploaded the same document {FileName}",doc.FileName);
                    throw new InvalidOperationException("Document already Exists");
                }
                var added = await _userDocRepository.Add(doc);
                _logger.LogInformation("Document {FileName} uploaded by user {UserId}",added.FileName, added.UploadedById);
                return added;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error while uploading document {FileName} by user {UserId}",doc.FileName, doc.UploadedById);
                throw;                              
            }
        }

        public async Task<ICollection<UserDocument>> GetAllUserDocs(Guid userId)
        {
            try
            {
                var docs = (await _userDocRepository.GetAll()).Where(d => d.UploadedById == userId && !d.IsDeleted)
                                                            .ToList();
                if (!docs.Any())
                {
                    _logger.LogWarning("No documents found for user {UserId}", userId);
                    throw new KeyNotFoundException("Documents not found");
                }

                _logger.LogInformation("Fetched {Count} documents for user {UserId}",docs.Count, userId);
                return docs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error fetching documents for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDocument> GetByFileName(string filename,string email)
        {
            try
            {
                var user = await _userService.GetUserByEmail(email);
                var doc  = (await _userDocRepository.GetAll()).SingleOrDefault(d =>
                                                                d.UploadedById == user.Id &&
                                                                d.FileName.Equals(filename, StringComparison.OrdinalIgnoreCase) &&
                                                                !d.IsDeleted);
                if (doc == null)
                {
                    _logger.LogWarning("Document {FileName} not found for uploader {Email}",filename, email);
                    throw new KeyNotFoundException("Document not found");
                }

                _logger.LogInformation("Document {FileName} fetched for uploader {Email}",filename, email);
                return doc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error fetching document {FileName} for uploader {Email}",filename, email);
                throw;
            }
        }

        public async Task<UserDocument> DeleteByFileName(string filename, Guid userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);

                var doc = (await _userDocRepository.GetAll()).SingleOrDefault(d =>
                                                            d.UploadedById == user.Id &&
                                                            d.FileName.Equals(filename, StringComparison.OrdinalIgnoreCase) &&
                                                            !d.IsDeleted);
                if (doc == null) throw new KeyNotFoundException("Document not found");

                doc.IsDeleted = true;
                var updated = await _userDocRepository.Update(doc.Id, doc);

                _logger.LogInformation("Document {FileName} soft-deleted by user {UserId}",filename, userId);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error deleting document {FileName} for user {UserId}",filename, userId);
                throw;
            }
        }
    }
}
