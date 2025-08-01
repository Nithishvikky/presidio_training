using Microsoft.AspNetCore.Mvc;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Services;

namespace ShopOnline.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("List")]
        public async Task<ActionResult> NewsList(int page = 1)
        {
            var news = await _newsService.NewsList(page);
            return Ok(news);
        }
    }
}