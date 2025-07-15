namespace DSS.Interfaces
{
    public interface IAzureBlobStorageService
    {
        Task UploadAsync(string fileName, byte[] fileData);
        Task<byte[]> DownloadAsync(string fileName);
        Task<bool> DeleteAsync(string fileName);
        Task<bool> ExistsAsync(string fileName);
    }
}