using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DSS.Interfaces;

namespace DSS.Services
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
        {
            _logger = logger;

            var vaultUri = configuration["AzureStorage:AccessUrl"];

            Console.WriteLine($"---START---\n{vaultUri}\n---END---");

            var secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
            var sasUrl = secretClient.GetSecret("SasUrl").Value.Value;

            _containerClient = new BlobContainerClient(new Uri(sasUrl));

            Console.WriteLine($"---START---\n{sasUrl}\n---END---");
        }

        public async Task UploadAsync(string fileName, byte[] fileData)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);

                using var stream = new MemoryStream(fileData);
                await blobClient.UploadAsync(stream, overwrite: true);

                _logger.LogInformation("File '{FileName}' uploaded to Azure Blob Storage.", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file '{FileName}' to Blob Storage.", fileName);
                throw;
            }
        }

        public async Task<byte[]> DownloadAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);

                if (!await blobClient.ExistsAsync())
                    throw new FileNotFoundException("File not found in blob storage", fileName);

                var response = await blobClient.DownloadStreamingAsync();
                using var memoryStream = new MemoryStream();
                await response.Value.Content.CopyToAsync(memoryStream);

                _logger.LogInformation("File '{FileName}' downloaded from Azure Blob Storage.", fileName);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file '{FileName}' from Blob Storage.", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(fileName);
                var result = await blobClient.DeleteIfExistsAsync();

                _logger.LogInformation("File '{FileName}' delete status: {Result}", fileName, result.Value);
                return result.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file '{FileName}' from Blob Storage.", fileName);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            return await blobClient.ExistsAsync();
        }
    }
}