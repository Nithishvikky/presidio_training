using Azure.Storage.Blobs;
using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Storage.Blobs;

namespace DSS.Services
{
    public class UserDocService : IUserDocService
    {
        private readonly IRepository<Guid, UserDocument> _userDocRepository;
        private readonly IUserService _userService;
        // private readonly IDocumentShareService _documentShareService;
        private readonly ILogger<UserDocService> _logger;
        // private readonly IAzureBlobStorageService _azureBlobStorageService;

        public UserDocService(IRepository<Guid, UserDocument> userDocRepository,
                              IUserService userService,
                              ILogger<UserDocService> logger)
                            //   IAzureBlobStorageService azureBlobStorageService)
        {
            _userDocRepository = userDocRepository;
            _userService = userService;
            // _documentShareService = documentShareService;
            _logger = logger;
            // _azureBlobStorageService = azureBlobStorageService;
        }

        public async Task<UserDocument> UploadDoc(UserDocument doc)
        {
            try
            {
                var documents = await _userDocRepository.GetAll();
                if (documents.Any(d => d.FileName.Equals(doc.FileName, StringComparison.OrdinalIgnoreCase) && d.UploadedById == doc.UploadedById && !d.IsDeleted))
                {
                    _logger.LogWarning("You already uploaded the same document {FileName}", doc.FileName);
                    throw new InvalidOperationException("Document already Exists");
                }
                var added = await _userDocRepository.Add(doc);

                // Azure
                // await _azureBlobStorageService.UploadAsync(doc.FileName, doc.FileData);

                _logger.LogInformation("Document {FileName} uploaded by user {UserId}", added.FileName, added.UploadedById);
                return added;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while uploading document {FileName} by user {UserId}", doc.FileName, doc.UploadedById);
                throw;
            }
        }

        public async Task<ICollection<UserDocument>> GetAllUserDocs(Guid userId)
        {
            try
            {
                var docs = (await _userDocRepository.GetAll()).Where(d => d.UploadedById == userId && !d.IsDeleted)
                                                            .ToList();
                // if (!docs.Any())
                // {
                //     _logger.LogWarning("No documents found for user {UserId}", userId);
                //     throw new KeyNotFoundException("Documents not found");
                // }

                _logger.LogInformation("Fetched {Count} documents for user {UserId}", docs.Count, userId);
                return docs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching documents for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDocument> GetByFileName(string filename,string email)
        {
            try
            {
                var user = await _userService.GetUserByEmail(email);
                var doc = (await _userDocRepository.GetAll()).SingleOrDefault(d =>
                                                                d.UploadedById == user.Id &&
                                                                d.FileName.Equals(filename, StringComparison.OrdinalIgnoreCase) &&
                                                                !d.IsDeleted);
                if (doc == null)
                {
                    _logger.LogWarning("Document {FileName} not found for uploader {Email}", filename, email);
                    throw new KeyNotFoundException("Document not found");
                }

                doc.UploadedByUser = user;

                // // Azure
                // doc.FileData = await _azureBlobStorageService.DownloadAsync(filename);

                _logger.LogInformation("Document {FileName} fetched for uploader {Email}", filename, email);

                return doc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document {FileName} for uploader {Email}", filename, email);
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

                // await _documentShareService.RevokeAllPermission(filename,user.Email);

                doc.IsDeleted = true;
                var updated = await _userDocRepository.Update(doc.Id, doc);

                // Azure
                // await _azureBlobStorageService.DeleteAsync(filename);

                _logger.LogInformation("Document {FileName} soft-deleted by user {UserId}", filename, userId);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {FileName} for user {UserId}", filename, userId);
                throw;
            }
        }

        public async Task<IEnumerable<UserDocument>> GetAllDocs(
            string? searchByEmail = null,
            string? searchFilename = null,
            string? sortBy = null,
            bool ascending = true
        )
        {
            try
            {
                var Alldoc = (await _userDocRepository.GetAll()).Where(d => !d.IsDeleted)
                                                            .AsQueryable();
                foreach (var doc in Alldoc)
                {
                    var user = await _userService.GetUserById(doc.UploadedById);
                    doc.UploadedByUser = user;
                }

                if (!string.IsNullOrEmpty(searchByEmail))
                {
                    Alldoc = Alldoc.Where(d => d.UploadedByUser.Email.Contains(searchByEmail));
                }

                if (!string.IsNullOrEmpty(searchFilename))
                {
                    Alldoc = Alldoc.Where(d => d.FileName.ToLower().Contains(searchFilename.ToLower()));
                }

                Alldoc = (sortBy?.ToLower()) switch
                {
                    "username" => ascending 
                        ? Alldoc.OrderBy(d => d.UploadedByUser.Username)
                        : Alldoc.OrderByDescending(d => d.UploadedByUser.Username),
                    "email" => ascending 
                        ? Alldoc.OrderBy(d => d.UploadedByUser.Email)
                        : Alldoc.OrderByDescending(d => d.UploadedByUser.Username),
                    "filename" => ascending 
                        ? Alldoc.OrderBy(d => d.FileName)
                        : Alldoc.OrderByDescending(d => d.FileName),

                    "uploadedat" => ascending 
                        ? Alldoc.OrderBy(d => d.UploadedAt)
                        : Alldoc.OrderByDescending(d => d.UploadedAt),

                    _ => Alldoc 
                };

                return Alldoc.ToList();
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Error fetching All documents");
                throw;
            }
        }
    }
}
