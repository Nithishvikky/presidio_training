using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface IColorService
    {
        public Task<Color?> GetColorById(int id);
        public Task<Color?> AddColor(Color color);
        public Task<bool> DeleteColor(int id);
        public Task<Color?> UpdateColor(Color color);
        public Task<ICollection<Color>> GetAllColorOrderByName();
    }
}