using Azure.Storage.Blobs;

namespace BlobAPI.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        public BlobStorageService(IConfiguration configuration)
        {
            var sasUrl = configuration["AzureBlob:ContainerSasUrl"];
            _containerClient = new BlobContainerClient(new Uri(sasUrl!));
        }

        public async Task UploadFile(Stream fileStream, string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        public async Task<Stream> DownloadFile(string fileName)
        {
            var blobClient = _containerClient?.GetBlobClient(fileName);
            if (await blobClient!.ExistsAsync())
            {
                var downloadInfor = await blobClient.DownloadStreamingAsync();
                return downloadInfor.Value.Content;
            }
            return null!;
        }
    }
}