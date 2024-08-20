using DertInfo.CrossCutting.Auth;
using DertInfo.CrossCutting.Configuration;
using DertInfo.CrossCutting.Connection;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.System;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using DertInfo.Services.Entity.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Groups
{
    public interface IGroupService
    {
        /// <summary>
        /// Lists all the groups in the system.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Only to be used by Dert Of Derts As we need to select the group.</remarks>
        Task<ICollection<Group>> ListAll();
        Task<ICollection<Group>> ListByUser();
        Task<GroupOverviewDO> GetOverview(int groupId);
        Task<GroupOverviewDO> GetOverviewActiveItemsOnly(int groupId);
        Task<ICollection<GroupMember>> GetMembers(int groupId);
        Task<ICollection<GroupImage>> GetImages(int groupId);
        Task<ICollection<Team>> GetTeams(int groupId);
        Task<ICollection<Registration>> GetRegistrations(int groupId);
        Task<Group> CreateMinimal(Group group);
        Task<Group> Create(Group group, byte[] imageByteArray, string imageExtension);
        Task<Group> UpdateOverview(Group myGroup);
        Task<GroupImage> AttachGroupImage(int groupId, byte[] imageByteArray, string imageExtension);
        Task<GroupImage> DetachGroupImage(int groupId, int groupImageId);
        
        Task<GroupMember> CreateMember(GroupMember myGroupMember);
        Task<GroupMember> RemoveMember(int groupMemberId);
        Task<Team> RemoveTeam(int teamId);
        Task<Team> CreateTeam(Team myTeam);

        Task<bool> ProcessMemberPermissionRequest(string pinCode);
        Task<int> GeneratePinCodesWhereEmpty();
        Task<Group> DeleteGroup(int groupId, bool hardDelete = false);
        Task ObfuscateGroupDataById(int groupId);
        Task<GroupAccessContext> GetUserAccessContext(int groupId);
        Task<Group> Configure(Group myGroup);
        Task<ContactInfoDO> GetContactInfo(int groupId);
        Task SetPrimaryGroupImage(int groupId, int groupImageId);
        
    }

    public class GroupService : IGroupService
    {
        
        IAuthService _authService;
        IGroupMemberRepository _groupMemberRepository;
        IRegistrationRepository _registrationRepository;
        ITeamRepository _teamRepository;
        IDertInfoUser _user;

        IGroupCreator _groupCreator;
        IGroupReader _groupReader;
        IGroupUpdater _groupUpdater;
        IGroupDeleter _groupDeleter;
        IGroupObfuscator _groupObfuscator;

        public GroupService(
            
            IAuthService authService,
            //IGroupMemberService groupMemberService,
            IGroupMemberRepository groupMemberRepository,
            //IImageRepository imageRepository,
            //IImageResizeService imageResizeService,
            //IImageService imageService,
            IDertInfoUser user,
            IRegistrationRepository registrationRepository,
            //ITeamService teamService,
            ITeamRepository teamRepository,
            // keep everything below here everything above should be offloaded
            IGroupCreator groupCreator,
            IGroupReader groupReader,
            IGroupUpdater groupUpdater,
            IGroupDeleter groupDeleter,
            IGroupObfuscator groupObfuscator
            )
        {
            _authService = authService;
            _groupMemberRepository = groupMemberRepository;
            _registrationRepository = registrationRepository;
            _teamRepository = teamRepository;
            _user = user;

            _groupCreator = groupCreator;
            _groupReader = groupReader;
            _groupUpdater = groupUpdater;
            _groupDeleter = groupDeleter;
            _groupObfuscator = groupObfuscator;
        }

        public async Task<ICollection<Group>> ListAll()
        {
            return await this._groupReader.ListAll();
        }

        public async Task<ICollection<Group>> ListByUser()
        {
            return await this._groupReader.ListByUser();
        }

        public async Task<GroupOverviewDO> GetOverview(int groupId)
        {
            return await this._groupReader.GetOverview(groupId);
        }

        public async Task<GroupOverviewDO> GetOverviewActiveItemsOnly(int groupId)
        {
            return await this._groupReader.GetOverviewActiveItemsOnly(groupId);
        }

        public async Task<ICollection<GroupMember>> GetMembers(int groupId)
        {
            var groupMembers = await _groupMemberRepository.GetByGroupWithAttendance(groupId);

            return groupMembers.OrderBy(gm => gm.Name).ToList();
        }

        public async Task<ICollection<GroupImage>> GetImages(int groupId)
        {
            return await this._groupReader.GetImages(groupId);
        }

        public async Task<ICollection<Team>> GetTeams(int groupId)
        {
            var teams = await this._teamRepository.GetByGroupWithImagesAndRegistrations(groupId);

            // If there is no team image then the group image should be used. 
            TeamImage groupFallbackImage = null;
            foreach (var team in teams)
            {
                if (team.TeamImages.Count == 0)
                {

                    if (groupFallbackImage == null)
                    {
                        var groupPrimaryImage = await this._groupReader.GetPrimaryImage(team.GroupId);
                        groupFallbackImage = new TeamImage
                        {
                            Id = 0,
                            TeamId = team.Id,
                            IsPrimary = true,
                            Image = groupPrimaryImage.Image,
                            ImageId = groupPrimaryImage.Id
                        };
                    }

                    team.TeamImages.Add(groupFallbackImage);
                }
            }

            return teams;
        }

        public async Task<ICollection<Registration>> GetRegistrations(int groupId)
        {
            var registrations = await this._registrationRepository.GetByGroupWithEventImagesAndCounts(groupId);

            return registrations;
        }

        public async Task<ContactInfoDO> GetContactInfo(int groupId)
        {
            return await this._groupReader.GetContactInfo(groupId);
        }

        public async Task<Group> CreateMinimal(Group group)
        {
            return await Create(group, null, null);
        }

        public async Task<Group> Create(Group group, byte[] imageByteArray, string imageExtension)
        {
            return await this._groupCreator.Create(group, imageByteArray, imageExtension);
        }

        public async Task<Group> UpdateOverview(Group updatedGroup)
        {
            return await this._groupUpdater.UpdateOverview(updatedGroup);
        }

        public async Task<GroupImage> AttachGroupImage(int groupId, byte[] imageByteArray, string imageExtension)
        {
            return await this._groupUpdater.AttachGroupImage(groupId, imageByteArray, imageExtension);
        }

        public async Task<GroupImage> DetachGroupImage(int groupId, int groupImageId)
        {
            return await this._groupUpdater.DetachGroupImage(groupId, groupImageId);
        }

        public async Task SetPrimaryGroupImage(int groupId, int groupImageId)
        {
            await this._groupUpdater.SetPrimaryGroupImage(groupId, groupImageId);
        }

        public async Task<GroupMember> CreateMember(GroupMember myGroupMember)
        {

            // note - this is to go to the member service

            myGroupMember.IsDeleted = false; // all creates are not deleted.
            myGroupMember = await _groupMemberRepository.Add(myGroupMember);
            return myGroupMember;
        }

        public async Task<Team> CreateTeam(Team myTeam)
        {
            // note - this is to go to the teams service

            // Set the new teams image to be the same as the primary for the group
            var groupPrimaryImage = await this._groupReader.GetPrimaryImage(myTeam.GroupId);
            var teamImage = new TeamImage();
            teamImage.ImageId = groupPrimaryImage.ImageId;
            teamImage.IsPrimary = true;

            myTeam.TeamImages = new List<TeamImage> { teamImage };

            myTeam.IsDeleted = false; // all creates are not deleted.
            myTeam = await _teamRepository.Add(myTeam);
            return myTeam;
        }

        public async Task<GroupMember> RemoveMember(int groupMemberId)
        {
            // note - this is to go to the member service

            var myGroupMember = await _groupMemberRepository.MarkDeleted(groupMemberId);
            return myGroupMember;
        }

        public async Task<Team> RemoveTeam(int teamId)
        {
            // note - this is to go to the teams service

            var myTeam = await _teamRepository.MarkDeleted(teamId);
            return myTeam;
        }

        public async Task<bool> ProcessMemberPermissionRequest(string pinCode)
        {
            // note - this is to go to the user service

            var group = await this._groupReader.GetOverviewByPinCode(pinCode);

            if (group != null)
            {
                // Apply the claim
                UserAccessClaims userAccessClaims = new UserAccessClaims();
                userAccessClaims.Auth0UserId = this._user.AuthId;
                userAccessClaims.GroupMemberPermissions = new string[] { group.Id.ToString() };
                userAccessClaims = await this._authService.AddAccessClaims(userAccessClaims);

                return userAccessClaims.GroupMemberPermissions.Contains(group.Id.ToString());
            }
            else
            {
                return false;
            }
        }

        public async Task<Group> DeleteGroup(int groupId, bool hardDelete = false)
        {
            if (hardDelete) 
            {
                return await _groupDeleter.HardDeleteGroup(groupId);
            }
            
            return await _groupDeleter.SoftDeleteGroup(groupId);
        }

        public async Task ObfuscateGroupDataById(int groupId)
        {
            await this._groupObfuscator.ObfuscateGroupDataById(groupId);
        }

        public async Task<GroupAccessContext> GetUserAccessContext(int groupId)
        {
            return await Task.Run(() => {

                var groupAdminIds = _user.ClaimsGroupAdmin != null ? _user.ClaimsGroupAdmin : new List<string>();
                var groupMemberIds = _user.ClaimsGroupMember != null ? _user.ClaimsGroupMember : new List<string>();

                if (groupAdminIds.Contains(groupId.ToString())) return GroupAccessContext.adminaccess;
                if (groupMemberIds.Contains(groupId.ToString())) return GroupAccessContext.memberaccess;

                // note - review this I believe that it is used by the client and therefore doesn't matter no server decitions are made based on this value.
                //      - however this reads strangely and potentially looks like a security concern. Provide clarification.
                //      - clarification - when the user gets the information on the client we need to know if its a group admin or member
                //      -               - we user this to determine routing.
                return GroupAccessContext.openaccess;
            });
            
        }

        public async Task<Group> Configure(Group myGroup)
        {
            return await this._groupUpdater.Configure(myGroup);
        }

        public async Task<int> GeneratePinCodesWhereEmpty()
        {
            return await this._groupCreator.GeneratePinCodesWhereEmpty();
        }
    }
}
