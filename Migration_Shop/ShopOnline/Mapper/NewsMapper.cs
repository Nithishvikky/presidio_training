using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using ShopOnline.DTOs;
using ShopOnline.Models;
using ShopOnline.Services;

namespace ShopOnline.Mapper
{
    public class NewsMapper
    {
        public News MapNews(NewsDto newsDto)
        {
            News news = new News();

            news.UserId = newsDto.UserId;
            news.Title = newsDto.Title;
            news.Image = newsDto.ImageUrl ?? "";
            news.ShortDescription = newsDto.ShortDescription ?? "";
            news.Content = newsDto.Content;
            news.CreatedDate = newsDto.CreatedDate;
            news.Status = newsDto.Status;

            return news;
        }
    }
}