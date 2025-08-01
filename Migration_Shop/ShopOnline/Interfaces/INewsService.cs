using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface INewsService
    {
        public Task<PagedResult<News>> NewsList(int? page);
        public Task<ICollection<News>> NewsAllList();
        public Task<News?> GetNewsById(int id);
        public Task<News?> AddNews(News news);
        public Task<News?> UpdateNews(News news);
        public Task<bool> DeleteNews(int id);
        public Task<String> ExportContentToCSV();
        public Task<byte[]> ExportContentToExcel();
    }
}