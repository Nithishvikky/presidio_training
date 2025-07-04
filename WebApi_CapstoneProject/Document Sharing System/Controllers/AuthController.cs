using Microsoft.AspNetCore.Mvc;
using DSS.Interfaces;
using DSS.Models;
using DSS.Models.DTOs;

namespace DSS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequestDto user)
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
            
            var auth = await _authService.LoginAsync(user);
            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Data = auth
            });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody]RefreshRequestDto refreshRequest)
        {
            var auth = await _authService.RefreshAsync(refreshRequest.RToken);
            return Ok(new ApiResponse<AuthResponse>
            {
                Success = true,
                Data = auth
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody]RefreshRequestDto refreshRequest)
        {
            await _authService.LogoutAsync(refreshRequest.RToken);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Data = "Logged out sucessfully"
            });
        }
    }
}