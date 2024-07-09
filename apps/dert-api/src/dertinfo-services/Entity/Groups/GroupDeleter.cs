using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.System;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Groups
{
    public interface IGroupDeleter
    {
        Task<Group> SoftDeleteGroup(int groupId);

        Task<Group> HardDeleteGroup(int groupId);
    }

    public class GroupDeleter : IGroupDeleter
    {
        IAuthService _authService;
        IGroupRepository _groupRepository;
        IRegistrationService _registrationService;
        IGroupUpdater _groupUpdater;
        IDertInfoUser _user;

        public GroupDeleter(
            IAuthService authService,
            IGroupRepository groupRepository,
            IRegistrationService registrationService,
            IGroupUpdater groupUpdater,
            IDertInfoUser user
            )
        {
            _groupRepository = groupRepository;
            _registrationService = registrationService;
            _groupUpdater = groupUpdater;
            _user = user;
            _authService = authService;
        }

        public async Task<Group> SoftDeleteGroup(int groupId)
        {
            var myGroup = await _groupRepository.MarkDeleted(groupId);
            var groupRegistrations = await _registrationService.ListForGroup(groupId);

            foreach (var registration in groupRegistrations)
            {
                // note - ideally we want to make use of the fact that we've already retrived the registration
                //      - so to reload it is a waste. We also don't want to have to reload it for the number of registrations
                //      - in the group and then process each one. We should create a registration deleter. 
                await this._registrationService.HandleGroupDeleted(registration.Id);
            }

            // Remove the claim
            UserAccessClaims userAccessClaims = new UserAccessClaims();
            userAccessClaims.Auth0UserId = this._user.AuthId;
            userAccessClaims.GroupPermissions = new string[] { myGroup.Id.ToString() };
            userAccessClaims = await this._authService.RemoveAccessClaims(userAccessClaims);

            return myGroup;
        }

        /// <summary>
        /// Hard delete group is permitted under the circumstances where the user creates a group in the first instance in error and then deletes the group.
        /// The group must not have any associated inforamtion that may have been shared with other parts of the system.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<Group> HardDeleteGroup(int groupId)
        {
            var myGroup = await _groupRepository.GetGroupForHardDeletion(groupId);

            // Check all group associations

            // All these Tests Must Pass
            var hardDeletePermitted = true;

            // Test Round 0
            if (!this._user.ClaimsGroupAdmin.Contains(groupId.ToString()))
            {
                hardDeletePermitted = false;
            }

            // Test Round 1
            hardDeletePermitted = myGroup.GroupImages.Count == 1 ? hardDeletePermitted : false;
            hardDeletePermitted = myGroup.GroupMembers.Count == 0 ? hardDeletePermitted : false;
            hardDeletePermitted = myGroup.Registrations.Count == 0 ? hardDeletePermitted : false;
            hardDeletePermitted = myGroup.Teams.Count == 1 ? hardDeletePermitted : false;

            // Test Round 2 - Team
            if (hardDeletePermitted)
            {
                hardDeletePermitted = myGroup.Teams.First().TeamAttendances.Count == 0 ? hardDeletePermitted : false;
                hardDeletePermitted = myGroup.Teams.First().TeamImages.Count == 1 ? hardDeletePermitted : false;
                hardDeletePermitted = myGroup.Teams.First().TeamName == myGroup.GroupName ? hardDeletePermitted : false;
            }

            if (hardDeletePermitted)
            {
                await _groupRepository.Delete(myGroup);

                // Remove the claim
                UserAccessClaims userAccessClaims = new UserAccessClaims();
                userAccessClaims.Auth0UserId = this._user.AuthId;
                userAccessClaims.GroupPermissions = new string[] { myGroup.Id.ToString() };
                userAccessClaims = await this._authService.RemoveAccessClaims(userAccessClaims);

                return myGroup;
            }

            throw new Exception("Hard Delete Failed As Not Permitted");

            
        }
    }
}
