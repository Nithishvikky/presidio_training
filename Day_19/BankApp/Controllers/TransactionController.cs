using Bank.Interfaces;
using Bank.Models;
using Bank.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> AddTransaction([FromBody] TransactionAddRequestDto transaction)
        {
            try
            {
                var newTransaction = await _transactionService.MadeTransaction(transaction);
                if (newTransaction != null)
                    return Created("", newTransaction);
                return BadRequest("Unable to process request at this moment");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet("accountId")]
        public async Task<ActionResult<ICollection<Transaction>>> GetTransations(int Id)
        {
            var result = await _transactionService.GetTransactionsByAccountNumber(Id);
            return Ok(result);
        }
    }
}