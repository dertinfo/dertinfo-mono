using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;

namespace DertInfo.Repository
{
    public interface IVenueRepository : IRepository<Venue, int>
    {
        Task<IEnumerable<Venue>> GetVenuesByCompetition(int competitionId);
        Task<Venue> GetVenueExpandedByJudgeSlots(int venueId);
    }

    public class VenueRepository : BaseRepository<Venue, int, DertInfoContext>, IVenueRepository
    {
        public VenueRepository(DertInfoContext context, IDertInfoUser user) : base(context, user) { }

        public async Task<Venue> GetVenueExpandedByJudgeSlots(int venueId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Venues
                .Where(v => v.Id == venueId)
                .Include(v => v.JudgeSlots).ThenInclude(js => js.Judge)
                .Include(v => v.JudgeSlots).ThenInclude(js => js.ScoreSet)
                .Include(v => v.CompetitionVenuesJoin);

                var venue = query.FirstOrDefault();

                return venue;
            });

            return await task;
        }

        public async Task<IEnumerable<Venue>> GetVenuesByCompetition(int competitionId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Venues
                .Where(v => v.CompetitionVenuesJoin.Any(vcj => vcj.CompetitionId == competitionId))
                .Include(v => v.JudgeSlots).ThenInclude(js => js.Judge)
                .Include(v => v.JudgeSlots).ThenInclude(js => js.ScoreSet);

                /*We need to remove from the result any judge slots that are related to the venue but not to the competition*/
                var venues = query.ToList();
                foreach (var venue in venues)
                {
                    venue.JudgeSlots = venue.JudgeSlots.Where(js => js.CompetitionId == competitionId).ToList();
                }

                return venues;
            });

            return await task;
        }
    }
}
