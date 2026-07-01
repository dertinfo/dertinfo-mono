using Microsoft.Extensions.Configuration;
using System;
using System.Text;


namespace DertInfo.CrossCutting.Connection
{
    /// <summary>
    /// An basic interface that faciliates the connection string and other things assosiated with connecting to an Azure storage account.
    /// - Must be dependency injected
    /// - Injection from container where the concrete implementation is configured at startup.
    /// </summary>
    public interface IStorageAccountConnection
    {
        string getImagesStorageConnectionString();
        string getScoreSheetsContainer();
        string getTeamPicturesContainer();
        string getEventPicturesContainer();
        string getDefaultPicturesContainer();
        string createScoreSheetFileName(int danceId, string fileExtension = "jpg");
        string createEventPictureFileName(int eventId, string fileExtension);
        //string createEventPictureFileName(string eventUuid, string fileExtension);
        string createTeamPictureFileName(int teamId, string fileExtension);
        // string createTeamPictureFileName(string teamUuid, string fileExtension);
        string createResizedImageFileName(string filename, string resizeTag);
        int getImageTagDimension(string resizeTag);
        string getOriginalsFolder();
        string get480x360Folder();
        string get100x100Folder();
    }

    public class StorageAccountConnection : IStorageAccountConnection
    {
        private IConfiguration _configuration;
        public StorageAccountConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string getImagesStorageConnectionString()
        {
            string storageProtocol = _configuration["StorageAccount:Images:Protocol"];
            string accountName = _configuration["StorageAccount:Images:Name"];
            string accountKey = _configuration["StorageAccount:Images:Key"]; 
            string blobEndpoint = _configuration["StorageAccount:Images:BlobEndpoint"];
            string tableEndpoint = _configuration["StorageAccount:Images:TableEndpoint"];
            string queueEndpoint = _configuration["StorageAccount:Images:QueueEndpoint"];

            // Example: DefaultEndpointsProtocol=https;AccountName=storagesample;AccountKey=<account-key>
            StringBuilder sb = new StringBuilder();
            sb.Append("DefaultEndpointsProtocol=" + storageProtocol + ";");
            sb.Append("AccountName=" + accountName + ";");
            sb.Append("AccountKey=" + accountKey + ";");
            if(blobEndpoint != string.Empty)
            {
                sb.Append("BlobEndpoint=" + blobEndpoint + ";");
            }
            if (tableEndpoint != string.Empty)
            {
                sb.Append("TableEndpoint=" + tableEndpoint + ";");
            }
            if (queueEndpoint != string.Empty)
            {
                sb.Append("QueueEndpoint=" + queueEndpoint + ";");
            }
            
            return sb.ToString();

        }

        public string getScoreSheetsContainer()
        {
            return "sheetimages";
        }

        public string getTeamPicturesContainer()
        {
            return "groupimages";
        }

        public string getEventPicturesContainer()
        {
            return "eventimages";
        }

        public string getDefaultPicturesContainer()
        {
            return "defaultimages";
        }

        public string getOriginalsFolder()
        {
            return "originals";
        }

        public string get480x360Folder()
        {
            return "480x360";
        }

        public string get100x100Folder()
        {
            return "100x100";
        }

        public string createScoreSheetFileName(int danceId, string fileExtension = "jpg")
        {
            // Clean the file extension
            if (fileExtension.Contains(".")) { fileExtension = fileExtension.Replace(".", string.Empty); }

            return "sheet-" + danceId + "-" + Guid.NewGuid() + "." + fileExtension;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="fileExtension">File extension not including the prefixed '.'</param>
        /// <returns></returns>
        public string createTeamPictureFileName(int groupId, string fileExtension)
        {
            // Clean the file extension
            if (fileExtension.Contains(".")) { fileExtension = fileExtension.Replace(".",string.Empty); }

            return "group-" + groupId + "-" + Guid.NewGuid() + "." + fileExtension.ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fileExtension">File extension not including the prefixed '.'</param>
        /// <returns></returns>
        public string createEventPictureFileName(int eventId, string fileExtension)
        {
            // Clean the file extension
            if (fileExtension.Contains(".")) { fileExtension = fileExtension.Replace(".", string.Empty); }

            return "event-" + eventId + "-" + Guid.NewGuid() + "." + fileExtension.ToLower();
        }

        public string createResizedImageFileName(string originalFilename, string resizeTag)
        {
            return resizeTag + "/" + originalFilename;
        }

        /// <summary>
        /// This should not be here. This is not direcly related to the storage account. And is here as the configuration is loaded here. 
        /// </summary>
        /// <param name="resizeTag"></param>
        /// <returns></returns>
        public int getImageTagDimension(string resizeTag)
        {
            string configurationKey = "ImageTagDimensions:" + resizeTag.ToLower();
            return int.Parse(_configuration[configurationKey]);
        }
    }
}
