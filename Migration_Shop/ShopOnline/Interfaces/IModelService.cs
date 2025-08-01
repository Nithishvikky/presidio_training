using ShopOnline.DTOs;
using ShopOnline.Models;

namespace ShopOnline.Interfaces
{
    public interface IModelService
    {
        public Task<Model?> GetModelById(int id);
        public Task<Model?> AddModel(Model model);
        public Task<bool> DeleteModel(int id);
        public Task<Model?> UpdateModel(Model model);
    }
}