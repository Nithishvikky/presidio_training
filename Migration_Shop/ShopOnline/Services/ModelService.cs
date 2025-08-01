using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;

namespace ShopOnline.Services
{
    public class ModelService : IModelService
    {
        private readonly ShopOnlineContext _context;
        public ModelService(ShopOnlineContext context)
        {
            _context = context;
        }
        public async Task<Model?> GetModelById(int id)
        {
            var Model = await _context.Models
                            .Include(c => c.Products)
                            .FirstOrDefaultAsync(c => c.ModelId == id);

            if (Model == null) 
                throw new KeyNotFoundException($"Model with ID {id} not found.");

            return Model;
        }

        public async Task<Model?> AddModel(Model Model)
        {
            _context.Models.Add(Model);
            await _context.SaveChangesAsync();
            return Model;
        }

        public async Task<bool> DeleteModel(int id)
        {
            var Model = await _context.Models.FindAsync(id);
            if (Model == null) return false;

            _context.Models.Remove(Model);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Model?> UpdateModel(Model Model)
        {
            var existing = await _context.Models.FindAsync(Model.ModelId);
            if (existing == null)
                throw new KeyNotFoundException($"Model with ID {Model.ModelId} not found.");

            existing.Model1 = Model.Model1;
            await _context.SaveChangesAsync();
            return existing;
        }
    }
}