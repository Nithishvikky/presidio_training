using ConsultingManagement.Interfaces;
using ConsultingManagement.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly IFileProcessingService _processingService;

    public SampleController(IFileProcessingService processingService)
    {

        _processingService = processingService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Doctor")]
    public ActionResult GetGreet()
    {
        return Ok("Hello World");
    }
    

    [HttpPost("FromCsv")]
    public async Task<IActionResult> BulkInsertFromCsv([FromBody] CsvUploadDto input)
    {
        return Ok(await _processingService.ProcessData(input));
    }
}