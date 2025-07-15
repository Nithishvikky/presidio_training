using Microsoft.AspNetCore.Mvc;
using DSS.Interfaces;
using DSS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DSS.Models.DTOs;
using DSS.Misc;


namespace DSS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserDocController : ControllerBase
    {
        private readonly IUserDocService _userDocService;
        private readonly IUserService _userService;
        private readonly IDocumentViewService _documentViewService;
        private readonly IDocumentShareService _documentShareService;
        public UserDocController(IUserDocService userDocService,
                                IDocumentViewService documentViewService,
                                IDocumentShareService documentShareService,
                                IUserService userService)
        {
            _userDocService = userDocService;
            _documentViewService = documentViewService;
            _documentShareService = documentShareService;
            _userService = userService;
        }

        [HttpPost("UploadDocument")]
        public async Task<ActionResult<UserDocument>> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Invalid file"
                });
            }

            if (file.Length > 10 * 1024 * 1024) // 10MB
            {
                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 413,
                    ErrorMessage = "File too large"
                });
            }

            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var userDocument = new UserDocument
            {
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileData = memoryStream.ToArray(),
                UploadedById = Guid.Parse(UserId)
            };

            userDocument = await _userDocService.UploadDoc(userDocument);

            return Ok(new ApiResponse<UserDocument>
            {
                Success = true,
                Data = userDocument
            });
        }

        [HttpGet("GetDocument")]
        [Authorize]
        public async Task<ActionResult<UserDocDetailDto>> DownloadFile(string filename, string UploaderEmail)
        {
            var userDocument = await _userDocService.GetByFileName(filename, UploaderEmail);

            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });


            if (role == "User") {
                if (!await _documentShareService.IsDocumentSharedWithUser(userDocument.Id, Guid.Parse(UserId)))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "File is not shared with you"
                    });
                }
            }
            await _documentViewService.LogDocumentView(userDocument.Id, Guid.Parse(UserId)); // added in audit table
            var mapper = new UserDocMapper();
            var docDetails = mapper.MapUserDoc(userDocument);
            docDetails.FileData = userDocument.FileData;

            return Ok(new ApiResponse<UserDocDetailDto>
            {
                Success = true,
                Data = docDetails
            });
        }

        [HttpGet("GetMyDocument")]
        [Authorize]
        public async Task<ActionResult<UserDocDetailDto>> DownloadMyFile(string filename)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var userDocument = await _userDocService.GetByFileName(filename, UserEmail);

            var mapper = new UserDocMapper();
            var docDetails = mapper.MapUserDoc(userDocument);
            docDetails.FileData = userDocument.FileData;

            return Ok(new ApiResponse<UserDocDetailDto>
            {
                Success = true,
                Data = docDetails
            });
        }

        [HttpGet("GetAllMyDocumentDetails")]
        [Authorize]
        public async Task<ActionResult<ICollection<UserDocDetailDto>>> MyDocumentDetails()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var userDocuments = await _userDocService.GetAllUserDocs(Guid.Parse(UserId));
            var viewHistory = await _documentViewService.GetUserViewHistory(Guid.Parse(UserId));

            var lastViewers = viewHistory
                            .GroupBy(v => v.FileName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.OrderByDescending(v => v.ViewedAt).FirstOrDefault()?.ViewerName
                            );
            var mapper = new UserDocMapper();
            var userDocDetails = userDocuments
                        .Select(doc =>
                        {
                            var dto = mapper.MapUserDoc(doc);
                            if (lastViewers.Any())
                            {
                                lastViewers.TryGetValue(doc.FileName, out var lastViewer);
                                dto.LastViewerName = lastViewer;
                            }
                            return dto;
                        }).ToList();

            return Ok(new ApiResponse<ICollection<UserDocDetailDto>>
            {
                Success = true,
                Data = userDocDetails
            });
        }

        [HttpGet("GetAllDocumentDetails")]
        public async Task<ActionResult<ICollection<UserDocDetailDto>>> AllDocumentDetails(
            string? userEmail = null,
            string? Filename = null,
            string? sortBy = null,
            bool ascending = true
        )
        {
            var userDocuments = await _userDocService.GetAllDocs(userEmail, Filename, sortBy, ascending);

            var mapper = new UserDocMapper();
            var userDocDetails = userDocuments
                        .Select(doc =>
                        {
                            var dto = mapper.MapUserDoc(doc);
                            dto.UploaderUsername = doc.UploadedByUser?.Username ?? "Unknown";
                            dto.UploaderEmail = doc.UploadedByUser?.Email ?? "Unknown";
                            return dto;
                        }).ToList();

            return Ok(new ApiResponse<ICollection<UserDocDetailDto>>
            {
                Success = true,
                Data = userDocDetails
            });
        }

        [HttpDelete("DeleteMyDocument")]
        [Authorize]
        public async Task<ActionResult> DeleteMyDocument(string fileName)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var userDocument = await _userDocService.DeleteByFileName(fileName, Guid.Parse(UserId));

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data = "Document Deleted sucessfully"
            });
        }
    }
}