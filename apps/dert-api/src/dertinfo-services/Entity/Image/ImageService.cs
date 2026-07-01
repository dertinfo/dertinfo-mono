using DertInfo.CrossCutting.Configuration;
using DertInfo.CrossCutting.Connection;
using DertInfo.Models.Database;
using DertInfo.Repository;
using EnsureThat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Images
{
    public interface IImageService
    {
        Task<IEnumerable<Image>> ListAll();
        Task<Image> FindById(int imageId);
        Task<Image> GetDefaultGroupImage();
        Task<Image> GetDefaultEventImage();
        Task MarkForDeletion(int imageId);

    }

    public class ImageService : IImageService
    {

        IImageRepository _imageRepository;
        IBlobStorageRepository _blobStorageRepository;
        IStorageAccountConnection _storageAccountConnection;
        IDertInfoConfiguration _dertInfoConfiguration;

        public ImageService(
            IImageRepository imageRepository,
            IDertInfoConfiguration dertInfoConfiguration,
            IBlobStorageRepository blobStorageRepository,
            IStorageAccountConnection storageAccountConnection
            )
        {
            _imageRepository = imageRepository;
            _blobStorageRepository = blobStorageRepository;
            _storageAccountConnection = storageAccountConnection;
            _dertInfoConfiguration = dertInfoConfiguration;
        }

        public async Task<IEnumerable<Image>> ListAll()
        {
            var myImages = await _imageRepository.GetAll();
            return myImages;
        }

        public async Task<Image> FindById(int imageId)
        {
            var myImage = await _imageRepository.GetById(imageId);
            return myImage;
        }

        public async Task<Image> GetDefaultGroupImage()
        {
            // Test to see if there is an image in the database with the default name
            var defaultImage = await _imageRepository.SingleOrDefault(i => i.BlobName.EndsWith(_dertInfoConfiguration.Defaults_GroupImageName));

            // Image is present in the database return the image
            if (defaultImage != null)
            {
                return defaultImage;
            }

            // Image is not present in the database create the image and sizes
            var container = _storageAccountConnection.getDefaultPicturesContainer();
            return await this.CreateDefaultImage(Properties.Resources.GroupDefaultImage, container, _dertInfoConfiguration.Defaults_GroupImageName, "png");
        }

        public async Task<Image> GetDefaultEventImage()
        {
            // Test to see if there is an image in the database with the default name
            var defaultImage = await _imageRepository.SingleOrDefault(i => i.BlobName.EndsWith(_dertInfoConfiguration.Defaults_EventImageName));

            // Image is present in the database return the image
            if (defaultImage != null)
            {
                return defaultImage;
            }

            // Image is not present in the database create the image and sizes
            var container = _storageAccountConnection.getDefaultPicturesContainer();
            return await this.CreateDefaultImage(Properties.Resources.EventDefaultImage, container, _dertInfoConfiguration.Defaults_EventImageName, "png");
        }

        /// <summary>
        /// Used to set a flag on images that are to be removed from blob storage. 
        /// This needs to be implemented as a batch process or run occationally. 
        /// The batch needs to ensure that there are no joins to the image before it can be deleted.
        /// The marked for deletion indicates that the image has been detached from a group as per not consent to gdpr. 
        /// However consent could be gained from another source.
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        public async Task MarkForDeletion(int imageId)
        {
            var myImage = await _imageRepository.GetById(imageId);

            // If the image is not the default one then mark it for deletion.
            if (!myImage.BlobName.EndsWith(_dertInfoConfiguration.Defaults_EventImageName) && !myImage.BlobName.EndsWith(_dertInfoConfiguration.Defaults_GroupImageName))
            {
                myImage.MarkedForRemoval = true;
                await _imageRepository.Update(myImage);
            }
        }

        private async Task<Image> CreateDefaultImage(byte[] embeddedResourceImage, string targetContainer, string staticImageName, string imageExtension)
        {
            Ensure.Any.IsNotNull(embeddedResourceImage, nameof(embeddedResourceImage), opts => opts.WithMessage("Embedded resource image is null"));
            Ensure.String.IsNotNullOrWhiteSpace(targetContainer);
            Ensure.String.IsNotNullOrWhiteSpace(staticImageName);

            var connectionString = this._storageAccountConnection.getImagesStorageConnectionString();
            var blobContainer = targetContainer;
            var blobPath = this._storageAccountConnection.getOriginalsFolder();
            var blobName = staticImageName;
            var blobExtension = imageExtension.Replace(".", string.Empty);

            // Create the blob for the image
            var blobUri = await this._blobStorageRepository.UploadFileToBlob(embeddedResourceImage, connectionString, blobContainer, blobPath, blobName);

            // Create a database reference to the image
            var defaultImageDbEntry = new Image()
            {
                Container = blobContainer,
                BlobPath = blobPath,
                BlobName = blobName,
                Extension = blobExtension,
                IsProtected = true
            };
            var image = await this._imageRepository.Add(defaultImageDbEntry);

            // Return the database reference to the image
            return image;
        }
    }
}
