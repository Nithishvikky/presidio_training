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
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IUserDocService _userDocService;


        public UserController(IUserService userService, IUserDocService userDocService)
        {
            _userService = userService;
            _userDocService = userDocService;

        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<User>> PostUser([FromBody] UserAddRequestDto user)
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
            var newUser = await _userService.AddUser(user);
            return Ok(new ApiResponse<User>
            {
                Success = true,
                Data = newUser
            });
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            var userDocs = await _userDocService.GetAllUserDocs(user.Id);
            user.UploadedDocuments = userDocs;
            return Ok(user);
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser(
            string? searchByEmail = null,
            string? searchByUsername = null,
            string? filterBy = null,
            string? sortBy = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10
        )
        {
            var users = await _userService.GetAllUsers(searchByEmail, searchByUsername, filterBy, sortBy, ascending, pageNumber, pageSize);
            return Ok(new ApiResponse<PagedResultDto<User>>
            {
                Success = true,
                Data = users
            });
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto passwordDto)
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

            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized(new ErrorObjectDto
                {
                    ErrorNumber = 401,
                    ErrorMessage = "Authentication required"
                });

            var user = await _userService.UpdateUserPassword(Guid.Parse(UserId), passwordDto);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data = "Password updated sucessfully"
            });
        }


        [HttpGet("GetInactiveUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetInactiveUsers([FromQuery] int days = 30)
        {
            var threshold = TimeSpan.FromDays(days);
            var users = await _userService.GetInactiveUsers(threshold);
            return Ok(new ApiResponse<IEnumerable<User>>
            {
                Success = true,
                Data = users
            });
        }

        
    }
}