using Microsoft.AspNetCore.Mvc;

namespace MyVMApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Please provide a name.");

            return Ok($"Hello {name}");
        }
    }
}
