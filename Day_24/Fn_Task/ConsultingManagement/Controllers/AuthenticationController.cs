using ConsultingManagement.Interfaces;
using ConsultingManagement.Models.DTOs;
using ConsultingManagement.Misc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
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

        [HttpPost]
        [CustomExceptionFilter]
        public async Task<ActionResult<UserLoginResponse>> UserLogin(UserLoginRequest loginRequest)
        {
            // try
            // {
            //     var result = await _authenticationService.Login(loginRequest);
            //     return Ok(result);
            // }
            // catch (Exception e)
            // {
            //     _logger.LogError(e.Message);
            //     return Unauthorized(e.Message);
            // }

            var result = await _authenticationService.Login(loginRequest);
            return Ok(result);
        }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleLoginCallback), "Authentication");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-login-callback")]
    public async Task<IActionResult> GoogleLoginCallback()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return Unauthorized();

        foreach (var cookie in Request.Cookies)
        {
            Console.WriteLine($"{cookie.Key}: {cookie.Value}");
        }

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return BadRequest("Email not found from Google");

        var loginResult = await _authenticationService.GoogleLogin(email);
        return Ok(loginResult);
    }

    }
}