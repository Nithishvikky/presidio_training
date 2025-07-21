using Microsoft.AspNetCore.Mvc;
using VT.Interface;
using VT.Models;

[ApiController]
[Route("api/videos")]
public class TrainingVideoController : ControllerBase
{
    private readonly ITrainingVideoService _videoService;

    public TrainingVideoController(ITrainingVideoService videoService)
    {
        _videoService = videoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _videoService.GetAllVideosAsync());

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadVideoRequestDto requestDto)
    {
        if (requestDto.File == null || requestDto.File.Length == 0)
            return BadRequest("No file uploaded.");

        await _videoService.UploadVideoAsync(requestDto.File, requestDto.Title, requestDto.Description);
        return Ok(new { message = "Uploaded successfully" });
    }
}
