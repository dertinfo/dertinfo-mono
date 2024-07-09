using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Groups
{
    public interface IGroupReader
    {
        /// <summary>
        /// Get a list of all the groups in the system.
        /// </summary>
        /// <returns>A list of all the groups in the system</returns>
        /// <remarks>Only to be used by Dert of Derts as we need to link have a select list of all groups so that we can link the group to get the images.</remarks>
        Task<ICollection<Group>> ListAll();
        Task<ICollection<Group>> ListByUser();
        Task<GroupOverviewDO> GetOverview(int groupId);
        Task<GroupOverviewDO> GetOverviewActiveItemsOnly(int groupId);
        Task<Group> GetOverviewByPinCode(string pinCode);
        Task<ContactInfoDO> GetContactInfo(int groupId);
        Task<ICollection<GroupImage>> GetImages(int groupId);
        Task<GroupImage> GetPrimaryImage(int groupId);
        
    }

    public class GroupReader : IGroupReader
    {
        IGroupRepository _groupRepository;
        IDertInfoUser _user;

        public GroupReader(
            IDertInfoUser user,
            IGroupRepository groupRepository

            )
        {
            _user = user;
            _groupRepository = groupRepository;

        }

        public async Task<ICollection<Group>> ListAll()
        {
            var groups = await _groupRepository.Find(g => !g.IsDeleted);

            return groups.ToList();
        }

        /// <summary>
        /// Should return all the groups that the user should have access to in the context of root access.
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<Group>> ListByUser()
        {
            if (!this._user.IsSuperAdmin)
            {

                var groupAdminIds = _user.ClaimsGroupAdmin != null ? _user.ClaimsGroupAdmin : new List<string>();
                var groupMemberIds = _user.ClaimsGroupMember != null ? _user.ClaimsGroupMember : new List<string>();
                var allGroupIds = groupAdminIds.Union(groupMemberIds).Distinct().Select(x => int.Parse(x)).ToArray();

                var groups = await _groupRepository.GetGroupsWithPrimaryImageByIds(allGroupIds);
                return groups.Where(g => g != null).ToList();
            }
            else
            {
                return await _groupRepository.GetAllWithPrimaryImage();
            }
        }

        public async Task<GroupOverviewDO> GetOverview(int groupId)
        {
            var group = await _groupRepository.GetGroupOverview(groupId);

            var outstandingInvoices = group.Registrations.SelectMany(r => r.Invoices).Where(i => !i.HasPaid);

            // note - could use automapper here.
            return new GroupOverviewDO {
                Id = group.Id,
                GroupBio = group.GroupBio,
                GroupName = group.GroupName,
                IsConfigured = group.IsConfigured,
                ContactName = group.PrimaryContactName,
                ContactTelephone = group.PrimaryContactNumber,
                GroupEmail = group.PrimaryContactEmail,
                GroupMemberJoiningPinCode = group.GroupMemberJoiningPinCode,
                MembersCount = group.GroupMembers.Count(),
                TeamsCount = group.Teams.Count(),
                OriginPostcode = group.OriginPostcode,
                OriginTown = group.OriginTown,
                OutstandingBalance = outstandingInvoices.Sum(oi => oi.InvoiceTotal),
                RegistrationsCount = group.Registrations.Count(),
                UnpaidInvoicesCount = outstandingInvoices.Count(),
                Visibility = group.GroupVisibilityType,
                GroupImages = group.GroupImages
            };
        }

        public async Task<GroupOverviewDO> GetOverviewActiveItemsOnly(int groupId)
        {
            var group = await _groupRepository.GetGroupOverview(groupId);

            // Restrict Registrations
            var activeRegistrations = group.Registrations.Where(r =>
                (r.FlowState == Models.System.Enumerations.RegistrationFlowState.New) ||
                (r.FlowState == Models.System.Enumerations.RegistrationFlowState.Submitted) ||
                (r.FlowState == Models.System.Enumerations.RegistrationFlowState.Confirmed)
            ).ToList();

            // Restrict Invoices
            var outstandingInvoices = group.Registrations.SelectMany(r => r.Invoices).Where(i => !i.HasPaid);

            return new GroupOverviewDO
            {
                Id = group.Id,
                GroupBio = group.GroupBio,
                GroupName = group.GroupName,
                IsConfigured = group.IsConfigured,
                ContactName = group.PrimaryContactName,
                ContactTelephone = group.PrimaryContactNumber,
                GroupEmail = group.PrimaryContactEmail,
                GroupMemberJoiningPinCode = group.GroupMemberJoiningPinCode,
                MembersCount = group.GroupMembers.Count(),
                TeamsCount = group.Teams.Count(),
                OriginPostcode = group.OriginPostcode,
                OriginTown = group.OriginTown,
                OutstandingBalance = outstandingInvoices.Sum(oi => oi.InvoiceTotal),
                RegistrationsCount = activeRegistrations.Count(),
                UnpaidInvoicesCount = outstandingInvoices.Count(),
                Visibility = group.GroupVisibilityType,
                GroupImages = group.GroupImages
            };
        }

        public async Task<Group> GetOverviewByPinCode(string pinCode)
        {
            var group = await _groupRepository.GetGroupByGroupMemberJoiningPinCode(pinCode);

            return group;
        }

        public async Task<ContactInfoDO> GetContactInfo(int groupId)
        {
            var contactInfo = await _groupRepository.GetContactInfo(groupId);

            return contactInfo;
        }

        public async Task<ICollection<GroupImage>> GetImages(int groupId)
        {
            var group = await this._groupRepository.GetGroupWithImagesById(groupId);

            return group.GroupImages;
        }

        public async Task<GroupImage> GetPrimaryImage(int groupId)
        {
            return await this._groupRepository.GetPrimaryImageForGroup(groupId);
        }

        
    }
}
