using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.System.Enumerations;

namespace DertInfo.Repository
{
    public interface ITeamRepository : IRepository<Team, int>
    {
        Task<ICollection<Team>> GetAllWithImages();
        Task<ICollection<Team>> GetAllWithImagesAndRegistrations();
        Task<ICollection<Team>> GetByGroupWithImages(int groupId);
        Task<ICollection<Team>> GetByGroupWithImagesAndRegistrations(int groupId);

        Task<Team> GetTeamWithImagesAndCountsById(int teamId);
        Task<Team> GetTeamDetail(int teamId);
        Task<Team> MarkDeleted(int teamId);

        Task<IEnumerable<Team>> GetByGroupAttendedEvent(int eventId, int groupId);
        Task<IEnumerable<Team>> GetByEventWithImages(int eventId, IEnumerable<RegistrationFlowState> flowStates);
    }

    public class TeamRepository : BaseRepository<Team, int, DertInfoContext>, ITeamRepository
    {
        public TeamRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<ICollection<Team>> GetAllWithImages()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Team> query = _context.Teams
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Team>> GetAllWithImagesAndRegistrations()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Team> query = _context.Teams
                .Include(t => t.TeamAttendances).ThenInclude(ta => ta.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(i => i.Image)
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Team>> GetByGroupWithImages(int groupId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Team> query = _context.Teams
                .Where(t => t.GroupId == groupId)
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Team>> GetByGroupWithImagesAndRegistrations(int groupId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Team> query = _context.Teams
                .Where(t => t.GroupId == groupId)
                .Include(t => t.TeamAttendances).ThenInclude(ta => ta.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(i => i.Image)
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<Team> GetTeamDetail(int id)
        {

            var task = Task.Run(() =>
            {
                // todo - determine what we actually need here.
                IQueryable<Team> query = _context.Teams
                .Where(t => t.Id == id)
                .Include(t => t.TeamAttendances).ThenInclude(ta => ta.TeamActivities).ThenInclude(ata => ata.Activity)
                .Include(t => t.TeamAttendances).ThenInclude(ta => ta.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(ei => ei.Image)
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<Team> GetTeamWithImagesAndCountsById(int teamId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Team> query = _context.Teams
                .Where(t => t.Id == teamId)
                .Include(t => t.TeamAttendances).ThenInclude(ta => ta.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(ei => ei.Image)
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<Team> MarkDeleted(int teamId)
        {

            var task = Task.Run(() =>
            {
                var team = _context.Teams.Find(teamId);

                team.IsDeleted = true;

                _context.SaveChanges();

                return team;
            });

            return await task;
        }



        public async Task<IEnumerable<Team>> GetByGroupAttendedEvent(int eventId, int groupId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Team> query = _context.Teams
                .Where(t => t.TeamAttendances.Any(ta => ta.Registration.EventId == eventId) && t.GroupId == groupId)
                .Include(t => t.TeamAttendances).ThenInclude(ta => ta.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(i => i.Image)
                .Include(t => t.TeamImages).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<IEnumerable<Team>> GetByEventWithImages(int eventId, IEnumerable<RegistrationFlowState> flowStates)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Teams
                .Where(t => t.TeamAttendances.Any(ta => ta.Registration.EventId == eventId && flowStates.Contains(ta.Registration.FlowState)))
                .Include(t => t.TeamImages.Where(ti => ti.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }
    }

}
