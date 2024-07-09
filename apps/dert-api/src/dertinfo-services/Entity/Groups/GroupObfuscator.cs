using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Repository;
using DertInfo.Services.Entity.Images;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Groups
{
    public interface IGroupObfuscator
    {
        Task ObfuscateGroupDataById(int groupId);
    }

    public class GroupObfuscator: IGroupObfuscator
    {
        IDertInfoConfiguration _dertInfoConfiguration;
        IGroupRepository _groupRepository;
        IGroupMemberService _groupMemberService;
        IGroupMemberRepository _groupMemberRepository;
        IImageService _imageService;
        ITeamService _teamService;

        public GroupObfuscator(
            IDertInfoConfiguration dertInfoConfiguration,
            IGroupRepository groupRepository,
            IGroupMemberService groupMemberService,
            IGroupMemberRepository groupMemberRepository,
            IImageService imageService,
            ITeamService teamService
            )
        {
            _dertInfoConfiguration = dertInfoConfiguration;
            _groupRepository = groupRepository;
            _groupMemberService = groupMemberService;
            _groupMemberRepository = groupMemberRepository;

            _imageService = imageService;
            _teamService = teamService;
        }

        public async Task ObfuscateGroupDataById(int groupId)
        {
            // note - we should ensure consent has not been directly gained by the member. This functionality is not yet available but the team admin will be the data owner until such a time that the member is bound.

            var group = await _groupRepository.GetGroupOverviewIncludeDeleted(groupId);

            group.PrimaryContactName = "GroupAdmin" + DertInfo.CrossCutting.Utilities.StringUtils.CreateStandardLengthNumeric(_dertInfoConfiguration.Constants_ObfuscationIdLength, groupId);
            group.PrimaryContactEmail = string.Empty;
            group.PrimaryContactNumber = string.Empty;

            // Members - ownership can change therefore handled within the service for the type.
            foreach (var gm in group.GroupMembers)
            {
                await _groupMemberService.ObfuscateMemberById(gm.Id);
            }

            foreach (var t in group.Teams)
            {
                await _teamService.ObfuscateTeamById(t.Id);
            }

            // Images - 
            foreach (var gi in group.GroupImages)
            {
                // Mark each image for deletion
                gi.IsPrimary = false;
                await _imageService.MarkForDeletion(gi.ImageId);
            }

            // Reattach the default image to the group.
            var defaultImage = await this._imageService.GetDefaultGroupImage();
            var defaultGroupImage = new GroupImage()
            {
                ImageId = defaultImage.Id,
                GroupId = group.Id,
                IsPrimary = true
            };

            group.GroupImages.Clear();
            group.GroupImages.Add(defaultGroupImage);

            await _groupRepository.Update(group);
        }
    }
}
