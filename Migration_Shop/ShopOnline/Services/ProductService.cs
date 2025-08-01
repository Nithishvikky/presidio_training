using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;

namespace ShopOnline.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopOnlineContext _context;
        public ProductService(ShopOnlineContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Product>> ProductListInPages(int? page, int? category)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;

            var products = _context.Products
                // .Include(p => p.Category)
                // .Include(p => p.Color)
                // .Include(p => p.Model)
                // .Include(p => p.User)
                .AsQueryable();

            if (category.HasValue)
            {
                products = products.Where(p => p.CategoryId == category);
            }
            var totalCount = await products.CountAsync();

            var productsList = await products
                .OrderByDescending(x => x.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = productsList,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Product?> ProductDetails(int id)
        {
            var Product = await _context.Products
                                .Include(p => p.Category)
                                .Include(p => p.Color)
                                .Include(p => p.Model)
                                .Include(p => p.User)
                                .Include(p => p.OrderDetails)
                                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (Product == null) throw new KeyNotFoundException($"Product with ID {id} not found.");

            return Product;
        }

        public async Task<Product?> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}