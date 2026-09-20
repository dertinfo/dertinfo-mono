using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace DertInfo.ImageResize.Services
{
    public interface IBlobWriter
    {
        Task WriteBlobStream(Stream inputStream, string containerName, string fileName);
    }

    internal class BlobWriter : IBlobWriter
    {
        private readonly IConfiguration _configuration;

        public BlobWriter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task WriteBlobStream(Stream inputStream, string containerName, string blobPathAndName)
        {
            var blobServiceClient = CreateBlobServiceClient();
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(blobPathAndName);

            inputStream.Position = 0;
            await blobClient.UploadAsync(inputStream, overwrite: true);
        }

        private BlobServiceClient CreateBlobServiceClient()
        {
            var connectionString = _configuration.GetValue<string>("StorageConnection:Images");
            if (!string.IsNullOrWhiteSpace(connectionString) && IsConnectionString(connectionString))
            {
                return new BlobServiceClient(connectionString);
            }

            var accountName = _configuration["StorageConnection:Images:accountName"]
                ?? _configuration["StorageConnection:Images__accountName"]
                ?? _configuration["StorageConnection__Images__accountName"];

            if (string.IsNullOrWhiteSpace(accountName))
            {
                throw new InvalidOperationException(
                    "StorageConnection:Images is not a connection string and StorageConnection:Images:accountName is missing.");
            }

            var blobUri = new Uri($"https://{accountName}.blob.core.windows.net");
            return new BlobServiceClient(blobUri, new DefaultAzureCredential());
        }

        private static bool IsConnectionString(string value) =>
            value.Contains("AccountKey=", StringComparison.OrdinalIgnoreCase)
            || value.Contains("UseDevelopmentStorage", StringComparison.OrdinalIgnoreCase)
            || value.Contains("DefaultEndpointsProtocol=", StringComparison.OrdinalIgnoreCase)
            || value.Contains("BlobEndpoint=", StringComparison.OrdinalIgnoreCase);
    }
}
