using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System;
using System.IO;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IBlobStorageRepository
    {
        Task<string> UploadFileToBlob(Byte[] file, string connection, string blobContainerName, string blobPath, string fileName);
        Task<bool> RemoveFileFromBlob(string storageConnectionString, string blobContainerName, string blobPath, string fileName);
        Task<Byte[]> RetriveFileFromBlob(string storageConnectionString, string blobContainerName, string blobPath, string fileName);
        Task<Uri> GetBaseUri(string storageConnectionString);
        Task<bool> TestExists(string storageConnectionString, string blobContainerName, string blobPath, string fileName);
    }

    public class BlobStorageRepository : IBlobStorageRepository
    {
        public BlobStorageRepository() { }

        public Task<Uri> GetBaseUri(string storageConnectionString)
        {
            return Task.Run(() => {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                return storageAccount.BlobEndpoint;
                // note - I'm not convinced that this is correct. 
            });
        }

        public async Task<string> UploadFileToBlob(Byte[] bytes, string storageConnectionString, string blobContainerName, string blobPath, string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container 
            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);

            // If container doesn’t exist, create it.
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null); //Note public access to blob level

            // Get a reference to a blob 
            var lookup = blobPath.Length > 0 ? $"{blobPath}/{fileName}" : fileName;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(lookup);

            // Create or overwrite the blob with the contents of a local file
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                await blockBlob.UploadFromStreamAsync(memoryStream);
            }

            return blockBlob.Uri.AbsoluteUri;
        }

        public async Task<bool> RemoveFileFromBlob(string storageConnectionString, string blobContainerName, string blobPath, string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container 
            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);

            // If container doesn’t exist, create it.
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            // Get a reference to a blob 
            var lookup = blobPath.Length > 0 ? $"{blobPath}/{fileName}" : fileName;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(lookup);

            // Delete the blob if it is existing
            return await blockBlob.DeleteIfExistsAsync();
        }

        public async Task<Byte[]> RetriveFileFromBlob(string storageConnectionString, string blobContainerName, string blobPath, string fileName)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container 
            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);

            // Get a reference to a blob 
            var lookup = blobPath.Length > 0 ? $"{blobPath}/{fileName}" : fileName;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(lookup);

            using (MemoryStream ms = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(ms);
                return ms.ToArray();
            }
        }

        public async Task<bool> TestExists(string storageConnectionString, string blobContainerName, string blobPath, string fileName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container 
            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);

            // Get a reference to a blob 
            var lookup = blobPath.Length > 0 ? $"{blobPath}/{fileName}" : fileName;
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(lookup);

            return await blockBlob.ExistsAsync();
        }
    }
}
