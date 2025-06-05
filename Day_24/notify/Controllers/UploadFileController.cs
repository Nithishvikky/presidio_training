using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notify.Interfaces;
using Notify.Models;
using Notify.Misc;
using Microsoft.AspNetCore.SignalR;

namespace Notify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadFileController : ControllerBase
    {
        private readonly IUploadFileService _fileService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public UploadFileController(IUploadFileService fileService,
                                    IHubContext<NotificationHub> hubContext)
        {
            _fileService = fileService;
            _hubContext = hubContext;
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file.");

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(username))
                return Unauthorized("User identity not found.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var uploadedFile = new UploadedFile
            {
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileData = memoryStream.ToArray(),
                UploadedByUsername = username
            };

            await _fileService.AddFile(uploadedFile);
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"A new document has been uploaded by {username} at {DateTime.UtcNow} Have look on it!!");

            return Ok("File uploaded successfully.");
        }
        
        [HttpGet("get/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = await _fileService.GetFile(id);
            if (file == null) return NotFound();

            return File(file.FileData, file.ContentType, file.FileName);
        }


    }
}