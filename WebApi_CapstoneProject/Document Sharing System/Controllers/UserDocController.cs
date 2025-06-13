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
        private readonly IDocumentViewService _documentViewService;
        private readonly IDocumentShareService _documentShareService;
        public UserDocController(IUserDocService userDocService,
                                IDocumentViewService documentViewService,
                                IDocumentShareService documentShareService)
        {
            _userDocService = userDocService;
            _documentViewService = documentViewService;
            _documentShareService = documentShareService;
        }

        [HttpPost("UploadDocument")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDocument>> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Invalid file"
                });

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

            return Created("", userDocument);
        }

        [HttpGet("GetDocument")]
        [Authorize]
        public async Task<ActionResult> DownloadFile(string filename, string UploaderEmail)
        {
            var userDocument = await _userDocService.GetByFileName(filename, UploaderEmail);

            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });


            if (!await _documentShareService.IsDocumentSharedWithUser(userDocument.Id, Guid.Parse(UserId)))
            {
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "File is not shared with you"
                });
            }
            await _documentViewService.LogDocumentView(userDocument.Id, Guid.Parse(UserId)); // added in audit table

            return File(userDocument.FileData!, userDocument.ContentType, userDocument.FileName);
        }

        [HttpGet("GetMyDocument")]
        [Authorize]
        public async Task<ActionResult> DownloadMyFile(string filename)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var userDocument = await _userDocService.GetByFileName(filename, UserEmail);

            return File(userDocument.FileData!, userDocument.ContentType, userDocument.FileName);
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
            var mapper = new UserDocMapper();
            var userDocDetails = userDocuments
                        .Select(doc => mapper.MapUserDoc(doc))
                        .ToList();

            return Ok(userDocDetails);
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
            
            return Ok("Document Deleted");
        }
    }
}