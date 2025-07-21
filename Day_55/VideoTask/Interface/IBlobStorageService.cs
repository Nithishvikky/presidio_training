
namespace VT.Interface
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}