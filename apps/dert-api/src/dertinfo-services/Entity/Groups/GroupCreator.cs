using DertInfo.CrossCutting.Auth;
using DertInfo.CrossCutting.Configuration;
using DertInfo.CrossCutting.Connection;
using DertInfo.Models.Database;
using DertInfo.Models.System;
using DertInfo.Repository;
using DertInfo.Services.Entity.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Groups
{
    public interface IGroupCreator
    {
        Task<Group> CreateMinimal(Group group);

        Task<Group> Create(Group group, byte[] imageByteArray, string imageExtension);

        Task<int> GeneratePinCodesWhereEmpty();
    }

    public class GroupCreator: IGroupCreator
    {
        IAuthService _authService;
        IBlobStorageRepository _blobStorageRepository;
        IGroupRepository _groupRepository;
        IImageRepository _imageRepository;
        IImageService _imageService;
        IStorageAccountConnection _storageAccountConnection;
        IDertInfoUser _user;

        public GroupCreator(
            IAuthService authService,
            IBlobStorageRepository blobStorageRepository,
            IGroupRepository groupRepository,
            IImageRepository imageRepository,
            IImageService imageService,
            IDertInfoUser user,  
            IStorageAccountConnection storageAccountConnection
            )
        {
            _authService = authService;
            _blobStorageRepository = blobStorageRepository;
            _groupRepository = groupRepository;
            _imageRepository = imageRepository;
            _imageService = imageService;
            _storageAccountConnection = storageAccountConnection;
            _user = user;
        }

        public async Task<Group> CreateMinimal(Group group)
        {
            return await Create(group, null, null);
        }

        /// <summary>
        /// Create a new group with the information provided
        /// As part of creating a group we also:
        /// - Create a team within the group of the same name
        /// - Add the group Uuid claim to the user creating the group
        /// - Handle the supplied image (if there is one) - Link to group & link to team
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<Group> Create(Group group, byte[] imageByteArray, string imageExtension)
        {
            // group.Uuid = Guid.NewGuid().ToString();

            // Upload image to Azure & Create reference in the database
            var image = new Image();
            if (imageByteArray != null && imageExtension != null)
            {
                var connectionString = _storageAccountConnection.getImagesStorageConnectionString();
                var container = _storageAccountConnection.getTeamPicturesContainer();
                var blobPath = _storageAccountConnection.getOriginalsFolder();
                var blobName = _storageAccountConnection.createTeamPictureFileName(group.Id, imageExtension);
                var blobExtension = imageExtension.Replace(".", string.Empty);

                var resourceUri = await _blobStorageRepository.UploadFileToBlob(imageByteArray, connectionString, container, blobPath, blobName);
                image.Container = container;
                image.BlobPath = blobPath;
                image.BlobName = blobName;
                image.Extension = blobExtension;
                image = await _imageRepository.Add(image);

            }
            else
            {
                // Apply the default group image to the group. 
                image = await _imageService.GetDefaultGroupImage();
                // note - automatically creates the sizes. Or will have already been created.
            }

            // Build the team 
            Team team = this.buildAutoAddedTeam(group.GroupName, group.GroupBio, image);

            // Assign the images
            var groupImages = new List<GroupImage>();
            if (image.Id != 0)
            {
                GroupImage groupImage = new GroupImage();
                groupImage.IsPrimary = true;
                groupImage.Image = image;
                groupImages.Add(groupImage);
            }

            group.Teams = new List<Team>() { team };
            group.GroupImages = groupImages;

            // Save and return
            group = await _groupRepository.Add(group);

            // Apply the claim
            UserAccessClaims userAccessClaims = new UserAccessClaims();
            userAccessClaims.Auth0UserId = this._user.AuthId;
            userAccessClaims.GroupPermissions = new string[] { group.Id.ToString() };
            userAccessClaims = await this._authService.AddAccessClaims(userAccessClaims);

            // Generate the membership pin codes
            await GeneratePinCodesWhereEmpty(); //note - this doesn't need to be awaited

            return group;
        }

        public async Task<int> GeneratePinCodesWhereEmpty()
        {
            var allGroups = await _groupRepository.GetAll();

            var usedPins = allGroups.Where(g => g.GroupMemberJoiningPinCode != null).Select(g => g.GroupMemberJoiningPinCode).ToList();
            var newAdditionsCount = 0;

            foreach (var g in allGroups.Where(g => g.GroupMemberJoiningPinCode == null))
            {

                var pinCodeAdded = await ApplyPinToGroup(g, usedPins);
                usedPins.Add(pinCodeAdded);
                newAdditionsCount++;
            }

            return newAdditionsCount;
        }

        private async Task<string> ApplyPinToGroup(Group g, List<string> usedPins)
        {
            var pinNumber = this.GeneratePinNumber();

            if (!usedPins.Any(up => up == pinNumber))
            {
                // Pin number is not in use
                g.GroupMemberJoiningPinCode = pinNumber;

                await _groupRepository.Update(g);

                return pinNumber;
            }
            else
            {
                // If pin number already in use then just try again.
                return await ApplyPinToGroup(g, usedPins);
            }
        }

        private string GeneratePinNumber()
        {
            Random generator = new Random();
            String r = generator.Next(0, 99999).ToString("D6");
            return r;
        }

        // note - this should be create default on the team service with some seed information.
        private Team buildAutoAddedTeam(string teamName, string teamBio, Image suppliedImage)
        {

            List<TeamImage> teamImages = new List<TeamImage>();

            if (suppliedImage.Id != 0)
            {
                TeamImage teamImage = new TeamImage();
                teamImage.IsPrimary = true;
                teamImage.Image = suppliedImage;
                teamImages.Add(teamImage);
            }

            Team team = new Team();
            team.TeamName = teamName;
            team.TeamBio = teamBio;
            team.TeamImages = teamImages;
            // team.Uuid = Guid.NewGuid().ToString();

            team = this.AppendTeamTrackingInformation(team);

            return team;
        }

        // todo - this shouldn't really be here. But is acceptable for now. We need to add the tracking information to the team object.
        private Team AppendTeamTrackingInformation(Team team)
        {
            team.DateCreated = DateTime.Now;
            team.CreatedBy = _user.AuthId;
            team.DateModified = DateTime.Now;
            team.ModifiedBy = _user.AuthId;

            return team;
        }
    }
}
