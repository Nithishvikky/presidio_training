using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Patient")]
    public ActionResult GetGreet()
    {
        return Ok("Hello World!!!");
    }
}