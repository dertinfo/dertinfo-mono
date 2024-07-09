using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace DertInfo.Util.Images
{
    public class ImageUploadAzure
    {
        #region Constructors

        public ImageUploadAzure(CloudBlobClient cloudBlobClient, string cloudBlobContainerName)
        {
            this.CloudBlobClient = cloudBlobClient;
            this.CloudBlobContainerName = cloudBlobContainerName;
        }

        #endregion

        #region Properties

        public CloudBlobClient CloudBlobClient { get; set; }
        public string CloudBlobContainerName { get; set; }

        #endregion

        /// <summary>
        /// Function to save a posted file to a cloud container if clash occours then increment the filename before the extention by a number.
        /// </summary>
        /// <param name="teamimage"></param>
        /// <param name="absFolderPath"></param>
        /// <returns></returns>
        public string SaveToFile(HttpPostedFileBase postedFile, string blobPrefix)
        {
            //Clean the upload name
            string cleanBlobPath = CleanAndValidateBlobPath(postedFile.FileName, blobPrefix);

            // Get a reference to the container
            CloudBlobContainer container = this.CloudBlobClient.GetContainerReference(this.CloudBlobContainerName);
            
            // Retrieve reference to a blob named "myblob"
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(cleanBlobPath);

            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadFromStream(postedFile.InputStream);

            return cleanBlobPath;
        }

        public string SaveToFile(string absfilePath, string blobPrefix)
        {
            string cleanBlobPath = CleanAndValidateBlobPath(absfilePath, blobPrefix);

            // Get a reference to the container
            CloudBlobContainer container = this.CloudBlobClient.GetContainerReference(this.CloudBlobContainerName);

            // Retrieve reference to a blob named "myblob"
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(cleanBlobPath);

            using (var fileStream = System.IO.File.OpenRead(absfilePath))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return cleanBlobPath;
        }

        public async Task<string> SaveToFileAsync(string absfilePath, string blobPrefix)
        {
            string cleanBlobPath = CleanAndValidateBlobPath(absfilePath, blobPrefix);

            // Get a reference to the container
            CloudBlobContainer container = this.CloudBlobClient.GetContainerReference(this.CloudBlobContainerName);

            // Retrieve reference to a blob named "myblob"
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(cleanBlobPath);

            using (var fileStream = System.IO.File.OpenRead(absfilePath))
            {
                Task upload = blockBlob.UploadFromStreamAsync(fileStream);
                await upload;
            }

            return cleanBlobPath;
        }

        private string CleanAndValidateBlobPath(string fileName, string blobPrefix)
        {
            string blobNameNoExt = System.IO.Path.GetFileNameWithoutExtension(fileName.Replace(" ", string.Empty));
            string blobExtension = System.IO.Path.GetExtension(fileName);
            string blobFullPath = blobPrefix + blobNameNoExt + blobExtension;

            int i = 0;
            bool hasBeenAppended = false;

            CloudBlobContainer container = this.CloudBlobClient.GetContainerReference(this.CloudBlobContainerName);

            bool blobExists = container.GetBlockBlobReference(blobFullPath).Exists();

            while (blobExists)
            {
                i++;
                if (hasBeenAppended)
                {
                    blobNameNoExt = blobNameNoExt.Substring(0, blobNameNoExt.Length - 1);
                }
                blobNameNoExt = blobNameNoExt + i.ToString();
                hasBeenAppended = true;
                blobFullPath = blobPrefix + blobNameNoExt + blobExtension;
                blobExists = container.GetBlockBlobReference(blobFullPath).Exists();
            }

            return blobFullPath;
        }
    }
}
