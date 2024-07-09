using DertInfo.CrossCutting.Configuration;
using DertInfo.CrossCutting.Connection;
using DertInfo.Models.Database;
using DertInfo.Repository;
using DertInfo.Services.Entity.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Groups
{
    public interface IGroupUpdater
    {
        Task<Group> UpdateOverview(Group myGroup);

        Task<GroupImage> DetachGroupImage(int groupId, int groupImageId);

        Task SetPrimaryGroupImage(int groupId, int groupImageId);

        Task<GroupImage> AttachGroupImage(int groupId, byte[] imageByteArray, string imageExtension);
        Task<Group> Configure(Group myGroup);
    }

    public class GroupUpdater : IGroupUpdater
    {
        IBlobStorageRepository _blobStorageRepository;
        IGroupRepository _groupRepository;
        IStorageAccountConnection _storageAccountConnection;
        IImageRepository _imageRepository;
        IImageService _imageService;
        ITeamService _teamService;

        public GroupUpdater(
            IBlobStorageRepository blobStorageRepository,
            IGroupRepository groupRepository,
            IImageRepository imageRepository,
            IImageService imageService,
            IStorageAccountConnection storageAccountConnection,
            ITeamService teamService
            )
        {
            _blobStorageRepository = blobStorageRepository;
            _groupRepository = groupRepository; 
            _imageRepository = imageRepository;
            _imageService = imageService;
            _storageAccountConnection = storageAccountConnection;
            _teamService = teamService;
        }

        public async Task<Group> UpdateOverview(Group updatedGroup)
        {
            var originalGroup = await _groupRepository.GetById(updatedGroup.Id);

            if (originalGroup == null) { throw new InvalidOperationException("Group Could Not Be Found"); }

            if (originalGroup.GroupName != updatedGroup.GroupName)
            {
                originalGroup.GroupName = updatedGroup.GroupName;
            }

            if (originalGroup.PrimaryContactEmail != updatedGroup.PrimaryContactEmail)
            {
                originalGroup.PrimaryContactEmail = updatedGroup.PrimaryContactEmail;
            }

            if (originalGroup.PrimaryContactName != updatedGroup.PrimaryContactName)
            {
                originalGroup.PrimaryContactName = updatedGroup.PrimaryContactName;
            }

            if (originalGroup.PrimaryContactNumber != updatedGroup.PrimaryContactNumber)
            {
                originalGroup.PrimaryContactNumber = updatedGroup.PrimaryContactNumber;
            }

            if (originalGroup.GroupBio != updatedGroup.GroupBio)
            {
                originalGroup.GroupBio = updatedGroup.GroupBio;
            }

            if (originalGroup.OriginTown != updatedGroup.OriginTown)
            {
                originalGroup.OriginTown = updatedGroup.OriginTown;
            }

            if (originalGroup.OriginPostcode != updatedGroup.OriginPostcode)
            {
                originalGroup.OriginPostcode = updatedGroup.OriginPostcode;
            }

            if (originalGroup.GroupVisibilityType != updatedGroup.GroupVisibilityType)
            {
                originalGroup.GroupVisibilityType = updatedGroup.GroupVisibilityType;
            }

            await _groupRepository.Update(originalGroup);

            return originalGroup;
        }

        public async Task<Group> Configure(Group myGroup)
        {
            var originalGroup = await _groupRepository.GetById(myGroup.Id);

            if (originalGroup == null) { throw new InvalidOperationException("Event Could Not Be Found"); }

            if (originalGroup.GroupBio != myGroup.GroupBio)
            {
                originalGroup.GroupBio = myGroup.GroupBio;
            }

            if (originalGroup.OriginTown != myGroup.OriginTown)
            {
                originalGroup.OriginTown = myGroup.OriginTown;
            }

            if (originalGroup.OriginPostcode != myGroup.OriginPostcode)
            {
                originalGroup.OriginPostcode = myGroup.OriginPostcode;
            }

            if (originalGroup.PrimaryContactName != myGroup.PrimaryContactName)
            {
                originalGroup.PrimaryContactName = myGroup.PrimaryContactName;
            }

            if (originalGroup.PrimaryContactEmail != myGroup.PrimaryContactEmail)
            {
                originalGroup.PrimaryContactEmail = myGroup.PrimaryContactEmail;
            }

            if (originalGroup.PrimaryContactNumber != myGroup.PrimaryContactNumber)
            {
                originalGroup.PrimaryContactNumber = myGroup.PrimaryContactNumber;
            }

            if (originalGroup.GroupVisibilityType != myGroup.GroupVisibilityType)
            {
                originalGroup.GroupVisibilityType = myGroup.GroupVisibilityType;
            }

            if (originalGroup.TermsAndConditionsAgreed != myGroup.TermsAndConditionsAgreed)
            {
                originalGroup.TermsAndConditionsAgreed = myGroup.TermsAndConditionsAgreed;
            }

            if (originalGroup.TermsAndConditionsAgreedBy != myGroup.TermsAndConditionsAgreedBy)
            {
                originalGroup.TermsAndConditionsAgreedBy = myGroup.TermsAndConditionsAgreedBy;
            }

            if (!originalGroup.IsConfigured)
            {
                originalGroup.IsConfigured = true;
            }

            // Save and return
            await _groupRepository.Update(originalGroup);

            return originalGroup;
        }

        public async Task<GroupImage> DetachGroupImage(int groupId, int groupImageId)
        {
            var group = await this._groupRepository.GetGroupWithImagesById(groupId);

            if (group.GroupImages != null)
            {
                var groupImage = group.GroupImages.FirstOrDefault(gi => gi.Id == groupImageId);
                if (groupImage != null)
                {

                    // note - it would be better here to just mark the group image as deleted. 

                    // question - does the removal of the image then prevent the save of the image deleted.

                    // if this is the primary image the primary image needs to be reallocated. 
                    if (groupImage.IsPrimary && group.GroupImages.Count() > 1)
                    {
                        var alternatePrimary = group.GroupImages.FirstOrDefault(gi => !gi.IsPrimary);
                        if (alternatePrimary != null)
                        {
                            await this.SetPrimaryGroupImage(groupId, alternatePrimary.Id);
                        }
                    }

                    if (group.GroupImages.Count() > 1)
                    {
                        // remove from the group images
                        group.GroupImages.Remove(groupImage);

                        // Mark the image as IsDeleted if not the default
                        var defaultImage = await _imageService.GetDefaultGroupImage();
                        if (groupImage.ImageId != defaultImage.Id)
                        {
                            groupImage.Image.IsDeleted = true;
                        }

                        // Save To Database - Apply update through group save.
                        bool done = await _groupRepository.Update(group);
                        return done ? groupImage : throw new Exception("Group Detach Image Failed");
                    }

                    throw new Exception("Cannot remove last image from group");
                }
            }

            return null;
        }

        public async Task<GroupImage> AttachGroupImage(int groupId, byte[] imageByteArray, string imageExtension)
        {
            var group = await this._groupRepository.GetGroupWithImagesById(groupId);


            var connection = _storageAccountConnection.getImagesStorageConnectionString();
            var blobContainer = _storageAccountConnection.getTeamPicturesContainer();
            var blobPath = _storageAccountConnection.getOriginalsFolder();
            var blobName = _storageAccountConnection.createTeamPictureFileName(groupId, imageExtension);
            var blobExtension = imageExtension.Replace(".", string.Empty);

            // Upload image to Azure
            var resourceUri = await _blobStorageRepository.UploadFileToBlob(imageByteArray, connection, blobContainer, blobPath, blobName);

            var image = new Image()
            {
                Container = blobContainer,
                BlobPath = blobPath,
                BlobName = blobName,
                Extension = blobExtension
            };

            image = await _imageRepository.Add(image);

            var groupImage = new GroupImage();
            groupImage.GroupId = groupId;
            groupImage.Image = image;

            if (group.GroupImages != null)
            {
                // if any images are the default image and a new image is added then remove the default image. 
                var defaultImage = await _imageService.GetDefaultGroupImage();
                var filteredImages = group.GroupImages.Where(gi => gi.ImageId != defaultImage.Id).ToList();
                group.GroupImages = filteredImages;

                // In the case where the new image is the only 1 set it as primary.
                if (filteredImages.Count == 0)
                {
                    // Set this as the primary group image
                    groupImage.IsPrimary = true;

                    // Ensure that we update the team with the new image and assign the image as the team primary
                    var teams = await this._teamService.GetByGroupWithImages(group.Id);
                    foreach (var team in teams)
                    {
                        var teamImage = new TeamImage();
                        teamImage.ImageId = image.Id;
                        teamImage.TeamId = team.Id;
                        teamImage.IsPrimary = true;

                        await this._teamService.AttachImage(teamImage);
                    }
                }

                // Add the uploaded image.
                group.GroupImages.Add(groupImage);
            }
            else
            {
                group.GroupImages = new List<GroupImage> { groupImage };
            }

            //Save To Database - Apply update through group save.
            bool done = await _groupRepository.Update(group);

            // We need to ensure that the rezises have been complated before returning.
            var resizePath = _storageAccountConnection.get480x360Folder();
            var loopCounter = 0;
            while (loopCounter < 8 && !await _blobStorageRepository.TestExists(connection, blobContainer, resizePath, blobName)) 
            {
                loopCounter++;
                Task.Delay(1000).Wait();
            }


            return done ? groupImage : throw new Exception("Group Attach Image Failed");
        }

        public async Task SetPrimaryGroupImage(int groupId, int groupImageId)
        {
            await this._groupRepository.ApplyPrimaryImage(groupId, groupImageId);
        }
    }
}
