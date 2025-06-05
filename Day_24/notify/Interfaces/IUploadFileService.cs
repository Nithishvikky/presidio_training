using Notify.Models;

namespace Notify.Interfaces
{
    public interface IUploadFileService
    {
        public Task<UploadedFile> AddFile(UploadedFile file);
        public Task<UploadedFile> GetFile(int id);
    }
}