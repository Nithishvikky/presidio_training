using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface ICategoryService
    {
        public Task<PagedResult<Category>> GetCategories(int? page);
        public Task<ICollection<Category>> GetCategoriesOrderByName();
        public Task<Category?> GetCategoryById(int id);
        public Task<Category?> UpdateCategory(Category category);
        public Task<bool> DeleteCategory(int id);
        public Task<Category?> AddCategory(Category category);
    }
}