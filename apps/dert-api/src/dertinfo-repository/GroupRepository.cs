using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IGroupRepository : IRepository<Group, int>
    {
        Task<ICollection<Group>> GetAllWithPrimaryImages();
        Task<ICollection<Group>> GetAllWithPrimaryImage();
        Task<ICollection<Group>> GetAllWithPrimaryImageAndCounts();
        Task<Group> GetGroupOverview(int id);
        Task<Group> GetGroupWithImagesById(int id);
        Task<Group> GetGroupWithPrimaryImageAndCountsById(int id);
        Task<ICollection<Group>> GetGroupsWithPrimaryImageByIds(int[] ids);
        Task<ICollection<Group>> GetGroupsWithPrimaryImageAndCountsByIds(int[] ids);
        Task ApplyPrimaryImage(int groupId, int groupImageId);
        Task<GroupImage> GetPrimaryImageForGroup(int groupId);
        Task<Group> GetGroupByGroupMemberJoiningPinCode(string pinCode);
        Task<ICollection<Group>> GetGroupsWithNoPin();
        Task<Group> MarkDeleted(int groupId);
        Task<Group> GetGroupOverviewIncludeDeleted(int id);
        Task<ContactInfoDO> GetContactInfo(int groupId);
        Task<Group> GetGroupForHardDeletion(int groupId);
    }

    public class GroupRepository : BaseRepository<Group, int, DertInfoContext>, IGroupRepository
    {
        public GroupRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<ICollection<Group>> GetAllWithPrimaryImages()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Group>> GetAllWithPrimaryImage()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Group>> GetAllWithPrimaryImageAndCounts()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Include(g => g.GroupMembers)
                .Include(g => g.Teams)
                .Include(g => g.Registrations)
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<Group> GetGroupOverview(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Where(g => g.Id == id)
                .Include(g => g.GroupMembers)
                .Include(g => g.Teams)
                .Include(g => g.Registrations).ThenInclude(r => r.Invoices)
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        /// <summary>
        /// Get the group with all the images
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is the call that is used to get the images for the select image screen.
        /// </remarks>
        public async Task<Group> GetGroupWithImagesById(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Where(g => g.Id == id)
                .Include(g => g.GroupImages).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<Group> GetGroupWithPrimaryImageAndCountsById(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Where(g => g.Id == id)
                .Include(g => g.GroupMembers)
                .Include(g => g.Teams)
                .Include(g => g.Registrations)
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<ICollection<Group>> GetGroupsWithPrimaryImageByIds(int[] ids)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Include(g => g.GroupMembers)
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query;
            });

            var results = await task;
            return results.Where(g => ids.Contains(g.Id)).ToList();
        }

        public async Task<ICollection<Group>> GetGroupsWithPrimaryImageAndCountsByIds(int[] ids)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Include(g => g.GroupMembers)
                .Include(g => g.Teams)
                .Include(g => g.Registrations)
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query;
            });

            var results = await task;
            return results.Where(g => ids.Contains(g.Id)).ToList();
        }

        public async Task<GroupImage> GetPrimaryImageForGroup(int groupId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<GroupImage> query = _context.GroupImages
                .Where(gi => gi.GroupId == groupId && gi.IsPrimary)
                .Include(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<ContactInfoDO> GetContactInfo(int groupId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Groups
                .Where(g => g.Id == groupId)
                .Select(g => new {
                    g.PrimaryContactName,
                    g.PrimaryContactEmail,
                    g.PrimaryContactNumber
                });

                var result = query.FirstOrDefault();

                return new ContactInfoDO
                {
                    ContactName = result.PrimaryContactName,
                    ContactEmail = result.PrimaryContactEmail,
                    ContactTelephone = result.PrimaryContactNumber,
                };
            });

            return await task;
        }

        public async Task ApplyPrimaryImage(int id, int groupImageId)
        {

            var task = Task.Run(() =>
            {
                _context.GroupImages
                    .Where(gi => gi.GroupId == id).ToList()
                    .ForEach(gi =>
                        gi.IsPrimary = gi.Id == groupImageId
                     );

                _context.SaveChanges();
            });

            await task;
        }

        public async Task<Group> GetGroupByGroupMemberJoiningPinCode(string pinCode)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Where(g => g.GroupMemberJoiningPinCode == pinCode);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<ICollection<Group>> GetGroupsWithNoPin()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups
                .Where(g => g.GroupMemberJoiningPinCode == null);

                return query.ToList();
            });

            return await task;
        }

        public async Task<Group> MarkDeleted(int groupId)
        {
            var task = Task.Run(() =>
            {
                var group = _context.Groups.Find(groupId);

                group.IsDeleted = true;

                _context.SaveChanges();

                return group;
            });

            return await task;
        }


        public async Task<Group> GetGroupOverviewIncludeDeleted(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups.IgnoreQueryFilters()
                .Where(g => g.Id == id)
                .Include(g => g.GroupMembers)
                .Include(g => g.Teams)
                .Include(g => g.Registrations)
                .Include(g => g.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<Group> GetGroupForHardDeletion(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Group> query = _context.Groups.IgnoreQueryFilters()
                .Where(g => g.Id == id)
                .Include(g => g.GroupMembers)
                .Include(g => g.Teams).ThenInclude(t => t.TeamImages)
                .Include(g => g.Teams).ThenInclude(t => t.TeamAttendances)
                .Include(g => g.Registrations)
                .Include(g => g.GroupImages);

                return query.FirstOrDefault();
            });

            return await task;
        }
    }
}
