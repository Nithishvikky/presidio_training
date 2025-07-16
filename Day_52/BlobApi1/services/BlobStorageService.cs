using Azure.Storage.Blobs;
using BlobAPI.Models;

namespace BlobAPI.Services
{
    public class BlobStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<BlobStorageService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        private async Task<BlobClient> GetBlobClientWithSas(string fileName)
        {
            //string functionUrl = $"https://dotnetfuncnithish.azurewebsites.net/api/generate-sas/{fileName}?code=bc94amQHaTSd5TQAMW5q5mZScGoTUP7SLoZ5zt868mi1AzFuRQZqrg==";
            var client = _httpClientFactory.CreateClient();
            var sasResponse = await client.GetAsync(functionUrl);
            if (!sasResponse.IsSuccessStatusCode)
            {
                var error = await sasResponse.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to get SAS URL: {error}");
                throw new InvalidOperationException("Could not obtain SAS URL.");
            }

            var sasData = await sasResponse.Content.ReadFromJsonAsync<SasResponse>();
            if (sasData == null || string.IsNullOrWhiteSpace(sasData.sasUrl))
            {
                throw new InvalidOperationException("SAS URL response invalid.");
            }

            _logger.LogInformation($"SAS URL obtained: {sasData.sasUrl}");

            // Create BlobClient directly using the SAS URL
            return new BlobClient(new Uri(sasData.sasUrl));
        }

        public async Task UploadFile(Stream fileStream, string fileName)
        {
            var blobClient = await GetBlobClientWithSas(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }

        public async Task<Stream> DownloadFile(string fileName)
        {
            var blobClient = await GetBlobClientWithSas(fileName);
            if (await blobClient.ExistsAsync())
            {
                var downloadInfo = await blobClient.DownloadStreamingAsync();
                return downloadInfo.Value.Content;
            }
            return null;
        }
    }
}