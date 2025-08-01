using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface IProductService
    {
        public Task<PagedResult<Product>> ProductListInPages(int? page, int? category);
        public Task<Product?> ProductDetails(int id);
        public Task<Product?> AddProduct(Product product);
    }
}