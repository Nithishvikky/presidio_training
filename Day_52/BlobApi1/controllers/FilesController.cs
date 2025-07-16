using BlobAPI.Models;
using BlobAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BlobAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;

        public FilesController(BlobStorageService blobStorageService)
        {
            _blobStorageService  = blobStorageService;
        }
        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _blobStorageService.DownloadFile(fileName);
            if (stream == null) 
                return NotFound();
            return File(stream, "application/octet-stream", fileName);
        }

        [Consumes("multipart/form-data")]
        
        [HttpPost("upload")]

        public async Task<IActionResult> Upload([FromForm] UploadRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file to upload");
            using var stream = request.File.OpenReadStream();
            await _blobStorageService.UploadFile(stream, request.File.FileName);
            return Ok("File uploaded");
        }
    }
}