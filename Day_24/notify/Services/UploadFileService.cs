using Notify.Interfaces;
using Notify.Models;


namespace Notify.Services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IRepository<int, UploadedFile> _uploadFileRepository;

        public UploadFileService(IRepository<int, UploadedFile> uploadFileRepository)
        {
            _uploadFileRepository = uploadFileRepository;
        }

        public async Task<UploadedFile> AddFile(UploadedFile file)
        {
            return await _uploadFileRepository.Add(file);
        }

        public async Task<UploadedFile> GetFile(int id)
        {
            return await _uploadFileRepository.Get(id);
        }
    }
}