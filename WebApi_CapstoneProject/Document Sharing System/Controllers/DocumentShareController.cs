using Microsoft.AspNetCore.Mvc;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DSS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentShareController : ControllerBase
    {

        private readonly IDocumentShareService _documentShareService;

        public DocumentShareController(IDocumentShareService documentShareService)
        {
            _documentShareService = documentShareService;
        }

        [HttpPost("GrantPermission")]
        public async Task<ActionResult<DocumentShare>> PostShare(string fileName, string ShareUserEmail)
        {

            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });
            var newShare = await _documentShareService.GrantPermission(fileName, UserEmail, ShareUserEmail);
            return Ok(new ApiResponse<DocumentShare>
            {
                Success = true,
                Data = newShare
            });
        }

        [HttpPost("GrantPermissionToUsers")]

        public async Task<ActionResult<DocumentShare>> PostShares(string fileName)
        {

            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });
            var newShares = await _documentShareService.GrantAllPermission(fileName, UserEmail);
            return Ok(new ApiResponse<ICollection<DocumentShare>>
            {
                Success = true,
                Data = newShares
            });
        }

        [HttpDelete("RevokePermission")]
        public async Task<ActionResult> DeleteShare(string fileName, string ShareUserEmail)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });
            await _documentShareService.RevokePermission(fileName, UserEmail, ShareUserEmail);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data = "Permission revoked"
            });
        }

        [HttpDelete("RevokePermissionToAll")]
        public async Task<ActionResult> DeleteShares(string fileName)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });
            await _documentShareService.RevokeAllPermission(fileName, UserEmail);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data = "Permission revoked for all"
            });
        }

        [HttpGet("GetFilesShared")]
        [Authorize]
        public async Task<ActionResult<ICollection<UserDocDetailDto>>> FilesSharedUser()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });
            var shares = await _documentShareService.GetFilesSharedWithUser(Guid.Parse(UserId));
            return Ok(new ApiResponse<ICollection<UserDocDetailDto>>
            {
                Success = true,
                Data = shares
            });
        }

        [HttpGet("GetSharedUsers")]
        [Authorize]
        public async Task<ActionResult<ICollection<SharedResponseeDto>>> FilesSharedUser(string fileName)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var shares = await _documentShareService.GetSharedUsersByFileName(fileName, UserEmail);

            return Ok(new ApiResponse<ICollection<SharedResponseeDto>>
            {
                Success = true,
                Data = shares
            });
        }

        [HttpGet("GetSharedUsersForAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ICollection<SharedResponseeDto>>> FilesSharedUserView(string fileName, string UploaderEmail)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(UserEmail))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var shares = await _documentShareService.GetSharedUsersByFileName(fileName, UploaderEmail);

            return Ok(new ApiResponse<ICollection<SharedResponseeDto>>
            {
                Success = true,
                Data = shares
            });
        }

        [HttpGet("GetDashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DashboardDto>> GetDashBoard()
        {

            var data = await _documentShareService.GetDashboard();

            return Ok(new ApiResponse<DashboardDto>
            {
                Success = true,
                Data = data
            });
        }

        [HttpGet("top-shared-documents")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTopSharedDocuments()
        {
            try
            {
                var topDocuments = await _documentShareService.GetTopSharedDocumentsAsync(5);
                return Ok(new ApiResponse<ICollection<TopSharedDocumentDto>>
                {
                    Success = true,
                    Data = topDocuments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Internal Server Error"
                });
            }
        }

    }
}