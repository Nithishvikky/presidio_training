using Microsoft.AspNetCore.Mvc;

namespace HelloSwaggerApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello Nithish!");
        }
    }
}
