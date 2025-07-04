using System.Security.Claims;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DSS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentViewController : ControllerBase
    {
        private readonly IDocumentViewService _documentViewService;

        public DocumentViewController(IDocumentViewService documentViewService)
        {
            _documentViewService = documentViewService;
        }

        [HttpGet("MyViewedDocs")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ViewerResponseDto>>> GetMyViewHistory()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var userId = Guid.Parse(userIdStr);

            var views = await _documentViewService.GetUserViewHistory(userId);

            return Ok(views);
        }

        [HttpGet("MyFileViewers")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ViewerResponseDto>>> GetMyFileViewHistory(string FileName)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var userId = Guid.Parse(userIdStr);

            var views = await _documentViewService.GetViewerHistoryByFileName(userId,FileName);
            
            return Ok(new ApiResponse<IEnumerable<ViewerResponseDto>>
            {
                Success = true,
                Data = views
            });
        }
    }
}