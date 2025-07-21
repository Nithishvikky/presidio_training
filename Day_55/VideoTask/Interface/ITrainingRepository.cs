
using VT.Models;

namespace VT.Interface
{
    public interface ITrainingVideoRepository
    {
        Task<IEnumerable<TrainingVideo>> GetAllAsync();
        Task<TrainingVideo?> GetByIdAsync(int id);
        Task AddAsync(TrainingVideo video);
    }
}
