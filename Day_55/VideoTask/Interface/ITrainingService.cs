using VT.Models;

namespace VT.Interface
{
    public interface ITrainingVideoService
    {
        Task<IEnumerable<TrainingVideo>> GetAllVideosAsync();
        Task<TrainingVideo?> GetVideoByIdAsync(int id);
        Task UploadVideoAsync(IFormFile file, string title, string description);
    }
}