using ConsultingManagement.Interfaces;
using ConsultingManagement.Models.DTOs;
using ConsultingManagement.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;



namespace ConsultingManagement.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationServiceCustom _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationServiceCustom authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        // [HttpPost]
        // [CustomExceptionFilter]
        // public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
        // {
        //     // try
        //     // {
        //     //     var result = await _authenticationService.Login(loginRequest);
        //     //     return Ok(result);
        //     // }
        //     // catch (Exception e)
        //     // {
        //     //     _logger.LogError(e.Message);
        //     //     return Unauthorized(e.Message);
        //     // }

        //     var result = await _authenticationService.Login(loginRequest);
        //     return Ok(result);
        // }

       [HttpGet]
        public async Task<IActionResult> LoginWithGoogle()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
 
            var properties = new AuthenticationProperties { RedirectUri = "/api/Authentication/google-callback" };
            properties.Items["prompt"] = "select_account";
 
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
 
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
 
            if (!result.Succeeded)
                return Unauthorized();
 
            var claims = _authenticationService.AuthenticateUser(result);
 
            return Ok(claims);
        }

    }
}