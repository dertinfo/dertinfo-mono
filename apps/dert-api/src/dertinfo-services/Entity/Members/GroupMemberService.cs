using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IGroupMemberService
    {
        Task<GroupMember> GetDetail(int groupMemberId);
        Task<GroupMember> UpdateMember(GroupMember groupMember);
        Task ObfuscateMemberById(int groupMemberId);
    }

    public class GroupMemberService : IGroupMemberService
    {
        IAuthService _authService;

        IDertInfoConfiguration _dertInfoConfiguration;

        IGroupMemberRepository _groupMemberRepository;

        public GroupMemberService(
            IAuthService authService,
            IDertInfoConfiguration dertInfoConfiguration,
            IGroupMemberRepository groupMemberRepository
            )
        {
            _authService = authService;
            _dertInfoConfiguration = dertInfoConfiguration;
            _groupMemberRepository = groupMemberRepository;
        }

        public async Task<GroupMember> GetDetail(int groupMemberId)
        {
            var groupMember = await _groupMemberRepository.GetGroupMemberDetail(groupMemberId);
            return groupMember;
        }

        public async Task<GroupMember> UpdateMember(GroupMember updatedGroupMember)
        {
            var originalMember = await _groupMemberRepository.GetById(updatedGroupMember.Id);

            if (originalMember == null) { throw new InvalidOperationException("Group Member Could Not Be Found"); }
            if (originalMember.GroupId != updatedGroupMember.GroupId) { throw new InvalidOperationException("It is not permitted to update a member across group"); }

            if (originalMember.Name != updatedGroupMember.Name)
            {
                originalMember.Name = updatedGroupMember.Name;
            }

            if (originalMember.EmailAddress != updatedGroupMember.EmailAddress)
            {
                originalMember.EmailAddress = updatedGroupMember.EmailAddress;
            }

            if (originalMember.TelephoneNumber != updatedGroupMember.TelephoneNumber)
            {
                if (updatedGroupMember.TelephoneNumber != null && updatedGroupMember.TelephoneNumber != string.Empty)
                {
                    // The users telephone number has been updated in this update
                    originalMember.TelephoneNumber = updatedGroupMember.TelephoneNumber;

                    /*
                     * We may need to change 2 factor auth here if was present. Or is integrated with whats app / twillo we may need to make changes here.
                     */
                }
            }

            if (originalMember.Facebook != updatedGroupMember.Facebook)
            {
                originalMember.Facebook = updatedGroupMember.Facebook;
            }

            if (originalMember.DateOfBirth != updatedGroupMember.DateOfBirth)
            {
                originalMember.DateOfBirth = updatedGroupMember.DateOfBirth;
            }

            if (originalMember.DateJoined != updatedGroupMember.DateJoined)
            {
                originalMember.DateJoined = updatedGroupMember.DateJoined;
            }

            if (originalMember.MemberType != updatedGroupMember.MemberType)
            {
                originalMember.MemberType = updatedGroupMember.MemberType;
            }

            if (originalMember.IsDeleted != updatedGroupMember.IsDeleted)
            {
                // We do not permit delete in an update call this has to be implemented explictly
            }

            await _groupMemberRepository.Update(originalMember);

            return originalMember;
        }

        public async Task ObfuscateMemberById(int groupMemberId)
        {
            // note - we should ensure consent has not been directly gained by the member. This functionality is not yet available but the team admin will be the data owner until such a time that the member is bound.

            var groupMember = await _groupMemberRepository.GetById(groupMemberId);

            groupMember.Name = "GroupMember" + DertInfo.CrossCutting.Utilities.StringUtils.CreateStandardLengthNumeric(_dertInfoConfiguration.Constants_ObfuscationIdLength, groupMember.Id);
            groupMember.TelephoneNumber = string.Empty;
            groupMember.EmailAddress = string.Empty;
            groupMember.Facebook = string.Empty;

            await _groupMemberRepository.Update(groupMember);
        }
    }
}
