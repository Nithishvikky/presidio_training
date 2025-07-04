
using DSS.Interfaces;
using DSS.Intrefaces;
using DSS.Misc;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace DSS.Services
{
    public class DocumentViewService : IDocumentViewService
    {
        private readonly IRepository<Guid,DocumentView> _documentViewRepo;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IRepository<Guid, UserDocument> _userDocRepo;
        private readonly IUserService _userService;
        private readonly ILogger<DocumentViewService> _logger;

        public DocumentViewService(IRepository<Guid, DocumentView> documentViewRepo,
                                    IHubContext<NotificationHub> hub,
                                    IRepository<Guid, UserDocument> userDocRepo,
                                    IUserService userService,
                                    ILogger<DocumentViewService> logger)
        {
            _documentViewRepo = documentViewRepo;
            _hub = hub;
            _userDocRepo = userDocRepo;
            _userService = userService;
            _logger = logger;
        }

        public async Task<DocumentView> LogDocumentView(Guid documentId, Guid userId)
        {
            try
            {
                var view = new DocumentView
                {
                    DocumentId = documentId,
                    ViewedByUserId = userId
                };

                var documentView = await _documentViewRepo.Add(view);

                var document = await _userDocRepo.Get(documentId);
                var viewer = await _userService.GetUserById(userId);
                var uploader = await _userService.GetUserById(document.UploadedById);

                _logger.LogInformation("Sending notification to group: {UploaderEmail}", uploader.Email);

                var viewerResponse = new ViewerResponseDto
                {
                    DocViewId = documentView.Id,
                    ViewerName = viewer.Username,
                    ViewerEmail = viewer.Email,
                    FileName = document.FileName,
                    ViewedAt = documentView.ViewedAt
                };
                await _hub.Clients.Group(uploader.Email).SendAsync("DocumentViewed",viewerResponse);

                return documentView;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LogDocumentView for documentId: {DocumentId}, userId: {UserId}", documentId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<ViewerResponseDto>> GetUserViewHistory(Guid userId)
        {
            try
            {

                var documents = await _userDocRepo.GetAll();
                var userDocIds = documents.Where(d => d.UploadedById == userId && !d.IsDeleted)
                                          .Select(d => d.Id).ToList();
                if (!userDocIds.Any())
                {
                    _logger.LogWarning("User {UserId} has no uploaded documents", userId);
                    throw new ArgumentNullException("No documents uploaded by this user.");
                }

                var views = await _documentViewRepo.GetAll();
                views = views.Where(v => userDocIds.Contains(v.DocumentId));

                // if (!views.Any())
                // {
                //     _logger.LogWarning("No views by {UserId}", userId);
                //     throw new ArgumentNullException("No views by the UserId");
                // }

                var viewerDtos = new List<ViewerResponseDto>();
                foreach (var v in views)
                {
                    var viewer = await _userService.GetUserById(v.ViewedByUserId);
                    var document = await _userDocRepo.Get(v.DocumentId);

                    viewerDtos.Add(new ViewerResponseDto
                    {
                        DocViewId = v.Id,
                        ViewerName = viewer?.Username ?? "Unknown",
                        FileName = document?.FileName ?? "Unknown",
                        ViewedAt = v.ViewedAt
                    });
                }
                return viewerDtos;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in LogDocumentView for {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ViewerResponseDto>> GetViewerHistoryByFileName(Guid userId, string FileName)
        {
            try
            {

                var documents = await _userDocRepo.GetAll();
                var document = documents.SingleOrDefault(d => d.UploadedById == userId && d.FileName.Equals(FileName,StringComparison.OrdinalIgnoreCase) && !d.IsDeleted);
                if (document == null)
                {
                    _logger.LogWarning("User {UserId} has no uploaded document {filename}", userId,FileName);
                    throw new ArgumentNullException("No document uploaded by this user.");
                }

                var views = await _documentViewRepo.GetAll();
                views = views.Where(v => v.DocumentId==document.Id);

                if (!views.Any())
                {
                    _logger.LogWarning("No views for the {file}", FileName);
                    throw new ArgumentNullException("No views for the {file}",FileName);
                }
                var groupedViews = views.GroupBy(v => v.ViewedByUserId)
                                        .Select(g => g.OrderByDescending(v => v.ViewedAt).First())
                                        .ToList();
                                        
                var viewerDtos = new List<ViewerResponseDto>();
                foreach (var v in groupedViews)
                {
                    var viewer = await _userService.GetUserById(v.ViewedByUserId);

                    viewerDtos.Add(new ViewerResponseDto
                    {
                        DocViewId = v.Id,
                        ViewerName = viewer?.Username ?? "Unknown",
                        FileName = document.FileName,
                        ViewedAt = v.ViewedAt
                    });
                }
                viewerDtos = viewerDtos.OrderByDescending(v => v.ViewedAt).ToList();
                return viewerDtos;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in LogDocumentView for {UserId}", userId);
                throw;
            }
        }
    }
}