using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Mapper;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class NewsManagementController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsManagementController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("List")]
        public async Task<ActionResult> NewsList()
        {
            var news = await _newsService.NewsAllList();
            return Ok(news);
        }

        [HttpPost]
        public async Task<IActionResult> AddNews([FromBody] NewsDto newsDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            var news = new NewsMapper().MapNews(newsDto);
            var created = await _newsService.AddNews(news);
            return CreatedAtAction(nameof(GetNewsDetails), new { id = created?.NewsId }, created);
        }

        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> UpdateNews(int id,[FromBody] NewsDto newsDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                Console.WriteLine(newsDto);
                Console.WriteLine(newsDto.Title);
                Console.WriteLine(newsDto.ShortDescription);
                Console.WriteLine(newsDto.Status);
                Console.WriteLine(newsDto.Content);
                Console.WriteLine(newsDto.ShortDescription);
                var news = new NewsMapper().MapNews(newsDto);
                news.NewsId = id;
                var updated = await _newsService.UpdateNews(news);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var result = await _newsService.DeleteNews(id);
            if (!result)
                return NotFound("News not found");

            return Ok("News removed successfully");
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> GetNewsDetails(int id)
        {
            try
            {
                var news = await _newsService.GetNewsById(id);
                return Ok(news);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportNewsToCsv()
        {
            var csvContent = await _newsService.ExportContentToCSV();
            var fileName = $"NewsListing_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(System.Text.Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportContentToExcel()
        {
            var excelBytes = await _newsService.ExportContentToExcel();
            var fileName = $"NewsListing_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}