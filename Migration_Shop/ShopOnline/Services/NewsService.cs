using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;
using ClosedXML.Excel;

namespace ShopOnline.Services
{
    public class NewsService : INewsService
    {
        private readonly ShopOnlineContext _context;
        public NewsService(ShopOnlineContext context)
        {
            _context = context;
        }

        public async Task<News?> AddNews(News news)
        {
            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return news;
        }

        public async Task<bool> DeleteNews(int id)
        {
             var news = await _context.News.FindAsync(id);
            if (news == null) return false;

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<News?> GetNewsById(int id)
        {
            var news = await _context.News
                        .Include(n => n.User)
                        .FirstOrDefaultAsync(n => n.NewsId == id);

            if (news == null)
                throw new KeyNotFoundException($"News with ID {id} not found.");

            return news;
        }

        public async Task<PagedResult<News>> NewsList(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 2;

            var News = _context.News
                    // .Include(n => n.User)
                    .OrderByDescending(n => n.NewsId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

            var totalCount = await _context.News.CountAsync();

            return new PagedResult<News>
            {
                Items = News,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ICollection<News>> NewsAllList()
        {

            var News = _context.News
                    .OrderByDescending(n => n.NewsId)
                    .ToList();
            return News;
        }

        public async Task<News?> UpdateNews(News news)
        {
            var existing = await _context.News.FindAsync(news.NewsId);
            if (existing == null)
            {
                Console.WriteLine($"News with ID {news.NewsId} not found.");
                throw new KeyNotFoundException($"News with ID {news.NewsId} not found.");
            }

            existing.Title = news.Title;
            existing.ShortDescription = news.ShortDescription;
            existing.CreatedDate = news.CreatedDate;
            existing.Status = news.Status;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<string> ExportContentToCSV()
        {
            var newsList = await _context.News
                .OrderBy(x => x.NewsId)
                .ToListAsync();

            var sb = new StringWriter();
            sb.WriteLine("\"NewsId\",\"Title\",\"ShortDescription\",\"CreatedDate\",\"Status\"");

            foreach (var news in newsList)
            {
                sb.WriteLine($"\"{news.NewsId}\",\"{news.Title}\",\"{news.ShortDescription}\",\"{news.CreatedDate:yyyy-MM-dd HH:mm:ss}\",\"{news.Status}\"");
            }

            return sb.ToString();
        }

        public async Task<byte[]> ExportContentToExcel()
        {
            var newsList = await _context.News.OrderBy(x => x.NewsId).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("News");
            worksheet.Cell(1, 1).Value = "NewsId";
            worksheet.Cell(1, 2).Value = "Title";
            worksheet.Cell(1, 3).Value = "ShortDescription";
            worksheet.Cell(1, 4).Value = "CreatedDate";
            worksheet.Cell(1, 5).Value = "Status";

            for (int i = 0; i < newsList.Count; i++)
            {
                var news = newsList[i];
                worksheet.Cell(i + 2, 1).Value = news.NewsId;
                worksheet.Cell(i + 2, 2).Value = news.Title;
                worksheet.Cell(i + 2, 3).Value = news.ShortDescription;
                worksheet.Cell(i + 2, 4).Value = news.CreatedDate;
                worksheet.Cell(i + 2, 5).Value = news.Status;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}