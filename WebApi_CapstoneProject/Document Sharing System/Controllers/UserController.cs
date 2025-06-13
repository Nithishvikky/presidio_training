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

        public UserController(IUserService userService)
        {
            _userService = userService;
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
            return Created("", newUser);
        }

        [HttpGet("GetUser")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            var user = await _userService.GetUserByEmail(email);
            return Ok(user);
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
        {
            var users = await _userService.GetAllUsersOnly();
            return Ok(users);
        }

        [HttpPut("ChangePassword")]
        [Authorize]
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
            return Ok("Password updated successfully");
        }
    }
}