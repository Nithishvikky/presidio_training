using Bank.Interfaces;
using Bank.Models;
using Bank.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<ActionResult<Account>> Addaccount([FromBody]AccountAddRequestDto account)
        {
            try
            {
                var newAccount = await _accountService.CreateAccount(account);
                if (newAccount != null)
                    return Created("", newAccount);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("accNumber")]
        public async Task<ActionResult<Account>> GetaccountByAccNumber(string accNumber)
        {
            var result = await _accountService.GetAccountByNumber(accNumber);
            return Ok(result);
        }
    }
}