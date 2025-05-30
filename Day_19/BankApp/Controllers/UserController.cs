using Bank.Interfaces;
using Bank.Models;
using Bank.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] UserAddRequestDto user)
        {
            try
            {
                var newUser = await _userService.RegisterUser(user);
                if (newUser != null)
                    return Created("", newUser);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<ICollection<User>>> GetUserByName(string name)
        {
            var result = await _userService.GetUserByName(name);
            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var result = await _userService.GetUserByEmail(email);
            return Ok(result);
        }

        [HttpGet("userId")]
        public async Task<ActionResult<ICollection<Account>>> GetAccountsByUserId(int Id)
        {
            var result = await _userService.GetAccounts(Id);
            return Ok(result);
        }
    }
}