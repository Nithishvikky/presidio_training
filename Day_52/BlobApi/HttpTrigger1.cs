using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Company.FunctionApp2;

public class Function
{
    private readonly ILogger<Function> _logger;

    public Function(ILogger<Function> logger)
    {
        _logger = logger;
    }

    [Function("Function")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "generate-sas/{blobName}")] HttpRequestData req,
        string blobName)
    {
        _logger.LogInformation($"Generating SAS for blob: {blobName}");

        string connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
        string containerName = Environment.GetEnvironmentVariable("ContainerName");
        string keyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri");

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(keyVaultUri))
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Missing required configuration settings.");
            return errorResponse;
        }

        // Create BlobServiceClient from connection string
        var blobServiceClient = new BlobServiceClient(connectionString);

        // Get account name from BlobServiceClient
        var accountName = blobServiceClient.AccountName;

        // Parse AccountKey safely by splitting (safe if you validate existence)
        string accountKey = null;
        foreach (var part in connectionString.Split(';'))
        {
            if (part.StartsWith("AccountKey=", StringComparison.OrdinalIgnoreCase))
            {
                accountKey = part.Substring("AccountKey=".Length);
                break;
            }
        }

        if (string.IsNullOrEmpty(accountKey))
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("AccountKey not found in connection string.");
            return errorResponse;
        }

        // Create credential
        var credential = new StorageSharedKeyCredential(accountName, accountKey);

        // Create BlobClient
        var blobClient = blobServiceClient.GetBlobContainerClient(containerName).GetBlobClient(blobName);

        // Build SAS
        DateTimeOffset expiresOn = DateTimeOffset.UtcNow.AddHours(1);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expiresOn
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);

        // Store SAS URL in Key Vault
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        string secretName = "myblobgayathri";

        var secretToStore = new KeyVaultSecret(secretName, sasUri.ToString())
        {
            Properties =
            {
                Tags = { { "ExpiresOn", expiresOn.UtcDateTime.ToString("o") } }
            }
        };

        await secretClient.SetSecretAsync(secretToStore);

        // Return SAS URL
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            sasUrl = sasUri.ToString(),
            expiresOn
        });

        return response;
    }
}