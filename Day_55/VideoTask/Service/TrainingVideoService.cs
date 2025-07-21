using VT.Interface;
using VT.Models;

namespace VT.Services
{
    public class TrainingVideoService : ITrainingVideoService
    {
        private readonly ITrainingVideoRepository _repository;
        private readonly IBlobStorageService _blobService;

        public TrainingVideoService(ITrainingVideoRepository repository, IBlobStorageService blobService)
        {
            _repository = repository;
            _blobService = blobService;
        }

        public async Task<IEnumerable<TrainingVideo>> GetAllVideosAsync() =>
            await _repository.GetAllAsync();

        public async Task<TrainingVideo?> GetVideoByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task UploadVideoAsync(IFormFile file, string title, string description)
        {
            string blobName = await _blobService.UploadAsync(file);

            var video = new TrainingVideo
            {
                Title = title,
                Description = description,
                BlobUrl = blobName
            };

            await _repository.AddAsync(video);
        }
    }
    
}