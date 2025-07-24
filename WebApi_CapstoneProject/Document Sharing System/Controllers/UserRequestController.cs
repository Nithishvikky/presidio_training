using Microsoft.AspNetCore.Mvc;
using DSS.Interfaces;
using DSS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DSS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UserRequestController : ControllerBase
    {
        private readonly IUserRequestService _userRequestService;
        private readonly ILogger<UserRequestController> _logger;

        public UserRequestController(IUserRequestService userRequestService, ILogger<UserRequestController> logger)
        {
            _userRequestService = userRequestService;
            _logger = logger;
        }

        [HttpPost("CreateRequest")]
        public async Task<ActionResult<UserRequestDto>> CreateRequest([FromBody] CreateUserRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 400,
                    ErrorMessage = string.Join(" | ", errorMessages)
                });
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var request = await _userRequestService.CreateRequestAsync(requestDto, Guid.Parse(userId));
                return Ok(new ApiResponse<UserRequestDto>
                {
                    Success = true,
                    Data = request
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 400,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create request");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to create request"
                });
            }
        }

        [HttpPut("ProcessRequest")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserRequestDto>> ProcessRequest([FromBody] ProcessUserRequestDto processDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 400,
                    ErrorMessage = string.Join(" | ", errorMessages)
                });
            }

            try
            {
                var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(adminUserId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var request = await _userRequestService.ProcessRequestAsync(processDto, Guid.Parse(adminUserId));
                return Ok(new ApiResponse<UserRequestDto>
                {
                    Success = true,
                    Data = request
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorObjectDto
                {
                    ErrorNumber = 400,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process request");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to process request"
                });
            }
        }

        [HttpGet("GetMyRequests")]
        public async Task<ActionResult<IEnumerable<UserRequestDto>>> GetMyRequests(int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var requests = await _userRequestService.GetUserRequestsAsync(Guid.Parse(userId), pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserRequestDto>>
                {
                    Success = true,
                    Data = requests
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user requests");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get user requests"
                });
            }
        }

        [HttpGet("GetAllRequests")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserRequestDto>>> GetAllRequests(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var requests = await _userRequestService.GetAllRequestsAsync(pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserRequestDto>>
                {
                    Success = true,
                    Data = requests
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all requests");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get all requests"
                });
            }
        }

        [HttpGet("GetPendingRequests")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserRequestDto>>> GetPendingRequests(int pageNumber = 1, int pageSize = 50)
        {
            try
            {
                var requests = await _userRequestService.GetPendingRequestsAsync(pageNumber, pageSize);
                return Ok(new ApiResponse<IEnumerable<UserRequestDto>>
                {
                    Success = true,
                    Data = requests
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending requests");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get pending requests"
                });
            }
        }

        [HttpGet("GetRequest/{requestId}")]
        public async Task<ActionResult<UserRequestDto>> GetRequest(Guid requestId)
        {
            try
            {
                var request = await _userRequestService.GetRequestByIdAsync(requestId);
                return Ok(new ApiResponse<UserRequestDto>
                {
                    Success = true,
                    Data = request
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ErrorObjectDto
                {
                    ErrorNumber = 404,
                    ErrorMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get request by ID: {RequestId}", requestId);
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get request"
                });
            }
        }

        [HttpGet("GetDocumentAccessStatus/{documentId}")]
        public async Task<ActionResult<DocumentAccessStatusDto>> GetDocumentAccessStatus(Guid documentId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var accessStatus = await _userRequestService.GetDocumentAccessStatusAsync(documentId, Guid.Parse(userId));
                return Ok(new ApiResponse<DocumentAccessStatusDto>
                {
                    Success = true,
                    Data = accessStatus
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get document access status for document: {DocumentId}", documentId);
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get document access status"
                });
            }
        }

        [HttpGet("GetMyAccessibleDocuments")]
        public async Task<ActionResult<IEnumerable<DocumentAccessStatusDto>>> GetMyAccessibleDocuments()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var accessibleDocuments = await _userRequestService.GetUserAccessibleDocumentsAsync(Guid.Parse(userId));
                return Ok(new ApiResponse<IEnumerable<DocumentAccessStatusDto>>
                {
                    Success = true,
                    Data = accessibleDocuments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get accessible documents");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to get accessible documents"
                });
            }
        }

        [HttpGet("HasDocumentAccess/{documentId}")]
        public async Task<ActionResult<bool>> HasDocumentAccess(Guid documentId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ErrorObjectDto
                    {
                        ErrorNumber = 401,
                        ErrorMessage = "Authentication required"
                    });
                }

                var hasAccess = await _userRequestService.HasDocumentAccessAsync(documentId, Guid.Parse(userId));
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Data = hasAccess
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check document access for document: {DocumentId}", documentId);
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to check document access"
                });
            }
        }

        [HttpPost("TriggerRearchive")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> TriggerRearchive()
        {
            try
            {
                var documentRearchiveService = HttpContext.RequestServices.GetRequiredService<IDocumentRearchiveService>();
                await documentRearchiveService.RearchiveExpiredDocumentsAsync();
                
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Data = "Document rearchive process completed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to trigger document rearchive");
                return StatusCode(500, new ErrorObjectDto
                {
                    ErrorNumber = 500,
                    ErrorMessage = "Failed to trigger document rearchive"
                });
            }
        }
    }
} 