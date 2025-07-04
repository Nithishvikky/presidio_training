using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notify.Interfaces;
using Notify.Models.DTOs;



namespace Notify.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class JWtAuthenticationController : ControllerBase
    {
        private readonly IJwtAuthenticationService _authenticationService;

        public JWtAuthenticationController(IJwtAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
        {
            try
            {
                var result = await _authenticationService.Login(loginRequest);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}