using Microsoft.EntityFrameworkCore;
using ShopOnline.Context;
using ShopOnline.DTOs;
using ShopOnline.Interfaces;
using ShopOnline.Models;

namespace ShopOnline.Services
{
    public class ColorService : IColorService
    {
        private readonly ShopOnlineContext _context;
        public ColorService(ShopOnlineContext context)
        {
            _context = context;
        }
        public async Task<Color?> GetColorById(int id)
        {
            var color = await _context.Colors
                            .Include(c => c.Products)
                            .FirstOrDefaultAsync(c => c.ColorId == id);

            if (color == null) 
                throw new KeyNotFoundException($"Color with ID {id} not found.");

            return color;
        }

        public async Task<Color?> AddColor(Color color)
        {
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            return color;
        }

        public async Task<bool> DeleteColor(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null) return false;

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Color?> UpdateColor(Color color)
        {
            var existing = await _context.Colors.FindAsync(color.ColorId);
            if (existing == null)
                throw new KeyNotFoundException($"Color with ID {color.ColorId} not found.");

            existing.Color1 = color.Color1;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<ICollection<Color>> GetAllColorOrderByName()
        {
            var colorList = await _context.Colors
                            // .Include(c => c.Products)
                            .OrderBy(c => c.ColorId)
                            .ToListAsync();
            return colorList;
        }
    }
}