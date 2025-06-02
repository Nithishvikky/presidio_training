using Microsoft.AspNetCore.Mvc;
using Bank.Services;
using Bank.Models.DTOs;

namespace Bank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotController : ControllerBase
    {
        private readonly ChatBotService _chatBotService;

        public ChatBotController(ChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] QesutionRequestDto request)
        {
            var reply = await _chatBotService.AskQuestion(request.Message);
            return Ok(new { reply });
        }
    }
}
