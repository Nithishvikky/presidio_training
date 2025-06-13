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
        [Authorize(Roles = "Admin")]
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
            return Created("", newShare);
        }

        [HttpPost("GrantPermissionToUsers")]
        [Authorize(Roles = "Admin")]

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
            return Created("", newShares);
        }

        [HttpDelete("RevokePermission")]
        [Authorize(Roles = "Admin")]
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
            return Ok("Permission Revoked");
        }

        [HttpDelete("RevokePermissionToAll")]
        [Authorize(Roles = "Admin")]
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
            return Ok("Permissions Revoked for all");
        }

        [HttpGet("GetFilesShared")]
        [Authorize]
        public async Task<ActionResult<ICollection<SharedResponseeDto>>> FilesSharedUser()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });
            var shares = await _documentShareService.GetFilesSharedWithUser(Guid.Parse(UserId));

            return Ok(shares);
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

            var shares = await _documentShareService.GetSharedUsersByFileName(fileName,UserEmail);

            return Ok(shares);
        }
    }
}