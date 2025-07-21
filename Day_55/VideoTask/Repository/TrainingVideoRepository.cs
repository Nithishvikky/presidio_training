// Repositories/TrainingVideoRepository.cs
using Microsoft.EntityFrameworkCore;
using VT.Context;
using VT.Interface;
using VT.Models;

namespace VT.Repository
{
    public class TrainingVideoRepository : ITrainingVideoRepository
    {
        private readonly AppDbContext _context;

        public TrainingVideoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrainingVideo>> GetAllAsync() =>
            await _context.TrainingVideos.ToListAsync();

        public async Task<TrainingVideo?> GetByIdAsync(int id) =>
            await _context.TrainingVideos.FindAsync(id);

        public async Task AddAsync(TrainingVideo video)
        {
            _context.TrainingVideos.Add(video);
            await _context.SaveChangesAsync();
        }
    }
}