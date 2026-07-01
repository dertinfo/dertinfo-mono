using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;

namespace DertInfo.Repository
{
    public interface IDanceRepository : IRepository<Dance, int>
    {
        Task<Dance> GetForAuthorization(int danceId);
        Task<Dance> GetDanceExpandedById(int id);
        Task<ICollection<Dance>> GetDancesExpandedByVenueId(int venueId);
        Task<ICollection<Dance>> GetDancesExpandedByCompetitionId(int activityId);
        Task<ICollection<Dance>> GetDancesExpandedByTeamIdAndCompetitionId(int teamId, int competitionId);
        Task<ICollection<Dance>> GetDancesExpandedByTeamIdAndVenueId(int teamId, int venueId);
    }

    public class DanceRepository : BaseRepository<Dance, int, DertInfoContext>, IDanceRepository
    {

        public DanceRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {
        }

        public async Task<Dance> GetForAuthorization(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Dance> query = _context.Dances
                .Where(d => d.Id == id)
                .Include(d => d.Venue);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<Dance> GetDanceExpandedById(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Dance> query = _context.Dances
                .Where(d => d.Id == id)
                .Include(d => d.TeamAttendance)
                .Include(d => d.TeamAttendance.Team).ThenInclude(t => t.TeamImages)
                .Include(d => d.Competition)
                .Include(d => d.Venue)
                .Include(d => d.MarkingSheetImages).ThenInclude(msi => msi.Image)
                .Include(d => d.DanceScores).ThenInclude(ds => ds.ScoreCategory).ThenInclude(sc => sc.ScoreSetScoreCategories);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<ICollection<Dance>> GetDancesExpandedByVenueId(int venueId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Dance> query = _context.Dances
                    .Where(d => d.VenueId == venueId)
                    .Include(d => d.TeamAttendance)
                    .Include(d => d.TeamAttendance.Team).ThenInclude(t => t.TeamImages)
                    .Include(d => d.Competition);


                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Dance>> GetDancesExpandedByTeamIdAndCompetitionId(int teamId, int competitionId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Dance> query = _context.Dances
                    .Where(d => d.TeamAttendance.TeamId == teamId && d.CompetitionId == competitionId)
                    .Include(d => d.TeamAttendance.Team).ThenInclude(t => t.TeamImages)
                    .Include(d => d.Competition)
                    .Include(d => d.Venue)
                    .Include(d => d.MarkingSheetImages).ThenInclude(msi => msi.Image)
                    .Include(d => d.DanceScores).ThenInclude(ds => ds.ScoreCategory);


                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Dance>> GetDancesExpandedByTeamIdAndVenueId(int teamId, int venueId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Dance> query = _context.Dances
                    .Where(d => d.TeamAttendance.TeamId == teamId && d.VenueId == venueId)
                    .Include(d => d.TeamAttendance.Team).ThenInclude(t => t.TeamImages)
                    .Include(d => d.Competition)
                    .Include(d => d.Venue)
                    .Include(d => d.MarkingSheetImages).ThenInclude(msi => msi.Image)
                    .Include(d => d.DanceScores).ThenInclude(ds => ds.ScoreCategory);


                return query.ToList();
            });

            return await task;
        }

        public async Task<ICollection<Dance>> GetDancesExpandedByCompetitionId(int competitionId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Dance> query = _context.Dances
                .Where(d => d.CompetitionId == competitionId)
                .Include(d => d.TeamAttendance).ThenInclude(ta => ta.CompetitionEntries).ThenInclude(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries).ThenInclude(_ => _.DertCompetitionEntryAttribute)
                .Include(d => d.TeamAttendance.Team)
                .Include(d => d.Competition)
                .Include(d => d.Venue).ThenInclude(d => d.JudgeSlots) // Was: .ThenInclude(js => js.Venue)
                .Include(d => d.DanceScores).ThenInclude(ds => ds.ScoreCategory);

                /* note -  
                 * After upgrade to EF Core 8 it started to throw an exception stating that "fix up" 
                 * will automatically include it. After making the change it seems to solve the problem 
                 * However I'm not fully convinced that it won't have a negative effect.
                 * note - 09/11/2024
                 * We had a further issue that after the first round of changes we completely broke the results pane by 
                 * not including the scores. We added those back in and in now appears to work correctly for results by ading
                 * the includes for them back in. 
                 */

                return query.ToList();
            });

            return await task;
        }
    }
}
