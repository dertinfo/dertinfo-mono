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
            return await this.GetOrEnsureDefaultImage(
                _dertInfoConfiguration.Defaults_GroupImageName,
                Properties.Resources.GroupDefaultImage);
        }

        public async Task<Image> GetDefaultEventImage()
        {
            return await this.GetOrEnsureDefaultImage(
                _dertInfoConfiguration.Defaults_EventImageName,
                Properties.Resources.EventDefaultImage);
        }

        /// <summary>
        /// Resolve the default group/event image, ensuring the blob exists in storage.
        /// Restored SQL + empty Azurite (or wiped storage) leaves a DB row with no blob;
        /// we re-upload the embedded resource in that case instead of returning a broken URL.
        /// </summary>
        private async Task<Image> GetOrEnsureDefaultImage(string defaultImageName, byte[] embeddedResourceImage)
        {
            var defaultImage = await _imageRepository.SingleOrDefault(i => i.BlobName.EndsWith(defaultImageName));
            var container = _storageAccountConnection.getDefaultPicturesContainer();

            if (defaultImage == null)
            {
                return await this.CreateDefaultImage(embeddedResourceImage, container, defaultImageName, "png");
            }

            var resolvedContainer = string.IsNullOrWhiteSpace(defaultImage.Container)
                ? container
                : defaultImage.Container;
            // Prefer DB path; if missing, use originals/ (same layout CreateDefaultImage writes).
            var resolvedBlobPath = string.IsNullOrWhiteSpace(defaultImage.BlobPath)
                ? _storageAccountConnection.getOriginalsFolder()
                : defaultImage.BlobPath;
            var blobName = defaultImage.BlobName;

            if (!await this.DefaultImageBlobExists(resolvedContainer, resolvedBlobPath, blobName))
            {
                await this.UploadDefaultImageBlob(embeddedResourceImage, resolvedContainer, resolvedBlobPath, blobName);

                // Keep the DB row aligned with where we just wrote the blob (common after old/null paths).
                var needsUpdate = false;
                if (defaultImage.Container != resolvedContainer)
                {
                    defaultImage.Container = resolvedContainer;
                    needsUpdate = true;
                }
                if (defaultImage.BlobPath != resolvedBlobPath)
                {
                    defaultImage.BlobPath = resolvedBlobPath;
                    needsUpdate = true;
                }
                if (!defaultImage.IsProtected)
                {
                    defaultImage.IsProtected = true;
                    needsUpdate = true;
                }
                if (needsUpdate)
                {
                    await _imageRepository.Update(defaultImage);
                }
            }

            return defaultImage;
        }

        private async Task<bool> DefaultImageBlobExists(string container, string blobPath, string blobName)
        {
            var connectionString = _storageAccountConnection.getImagesStorageConnectionString();
            return await _blobStorageRepository.TestExists(
                connectionString,
                container,
                blobPath ?? string.Empty,
                blobName);
        }

        private async Task UploadDefaultImageBlob(byte[] embeddedResourceImage, string blobContainer, string blobPath, string blobName)
        {
            Ensure.Any.IsNotNull(embeddedResourceImage, nameof(embeddedResourceImage), opts => opts.WithMessage("Embedded resource image is null"));
            Ensure.String.IsNotNullOrWhiteSpace(blobContainer);
            Ensure.String.IsNotNullOrWhiteSpace(blobName);

            var connectionString = _storageAccountConnection.getImagesStorageConnectionString();
            await _blobStorageRepository.UploadFileToBlob(embeddedResourceImage, connectionString, blobContainer, blobPath ?? string.Empty, blobName);
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

            var blobPath = this._storageAccountConnection.getOriginalsFolder();
            var blobExtension = imageExtension.Replace(".", string.Empty);

            await this.UploadDefaultImageBlob(embeddedResourceImage, targetContainer, blobPath, staticImageName);

            // Create a database reference to the image
            var defaultImageDbEntry = new Image()
            {
                Container = targetContainer,
                BlobPath = blobPath,
                BlobName = staticImageName,
                Extension = blobExtension,
                IsProtected = true
            };
            var image = await this._imageRepository.Add(defaultImageDbEntry);

            // Return the database reference to the image
            return image;
        }
    }
}
