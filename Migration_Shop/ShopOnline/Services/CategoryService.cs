using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;

namespace ShopOnline.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ShopOnlineContext _context;
        public CategoryService(ShopOnlineContext context)
        {
            _context = context;
        }

        public async Task<Category?> AddCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Category>> GetCategories(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;

            var Categories = _context.Categories
                            .Include(c => c.Products)
                            .OrderBy(c => c.Name)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            var totalCount = await _context.Categories.CountAsync();

            return new PagedResult<Category>
            {
                Items = Categories,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ICollection<Category>> GetCategoriesOrderByName()
        {
            var categoryList = await _context.Categories
                            // .Include(c => c.Products)
                            .OrderBy(c => c.Name)
                            .ToListAsync();
            return categoryList;
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            var category = await _context.Categories
                            .Include(c => c.Products)
                            .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) 
                throw new KeyNotFoundException($"Category with ID {id} not found.");

            return category;
        }

        public async Task<Category?> UpdateCategory(Category category)
        {
            var existing = await _context.Categories.FindAsync(category.CategoryId);
            if (existing == null)
                throw new KeyNotFoundException($"Category with ID {category.CategoryId} not found.");

            existing.Name = category.Name;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}