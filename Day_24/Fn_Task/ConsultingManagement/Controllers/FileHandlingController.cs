using Microsoft.AspNetCore.Mvc;

namespace ConsultingManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileHandlingController : ControllerBase
    {
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

        public FileHandlingController()
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(_storagePath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { FileName = file.FileName, Message = "File uploaded successfully." });
        }
        
        [HttpGet("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            var filePath = Path.Combine(_storagePath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", fileName);
            // return Ok(bytes);
        }
    }
}