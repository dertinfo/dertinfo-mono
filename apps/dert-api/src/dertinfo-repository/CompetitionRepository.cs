using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.System.Enumerations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface ICompetitionRepository : IRepository<Competition, int>
    {
        Task<Competition> GetCompetitionExpandedById(int competitionId);
        Task<IEnumerable<Competition>> GetAllForResultsLookup();
        Task<IEnumerable<Competition>> GetMany(IEnumerable<int> competitionIds);
        Task<CompetitionSummaryDO> GetForSummary(int competitionId);
        Task<Competition> ClearCompetitionAttendance(int competitionId);
    }

    public class CompetitionRepository : BaseRepository<Competition, int, DertInfoContext>, ICompetitionRepository
    {
        public CompetitionRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<Competition> GetCompetitionExpandedById(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Competition> query = _context.Competitions
                .Where(c => c.Id == id)
                .Include(c => c.ScoreCategories);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<IEnumerable<Competition>> GetMany(IEnumerable<int> competitionIds)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Competition> query = _context.Competitions
                .Where(c => competitionIds.Contains(c.Id))
                .Include(c => c.CompetitionVenuesJoin).ThenInclude(cv => cv.Venue)
                .Include(c => c.CompetitionEntries)
                .Include(c => c.CompetitionJudges)
                .Include(c => c.JudgeSlots).ThenInclude(js => js.Judge);

                return query.ToList();
            });

            return await task;
        }

        public async Task<IEnumerable<Competition>> GetAllForResultsLookup()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Competition> query = _context.Competitions
                .Include(c => c.Event).ThenInclude(e => e.EventSettings);

                return query.ToList();
            });

            return await task;
        }

        public async Task<CompetitionSummaryDO> GetForSummary(int competitionId)
        {
            var task = Task.Run(() =>
            {
                var activityQuery = _context.ActivityTeamAttendances.Where(ata => ata.Activity.CompetitionId == competitionId && (ata.TeamAttendance.Registration.FlowState == RegistrationFlowState.Confirmed || ata.TeamAttendance.Registration.FlowState == RegistrationFlowState.Closed));

                var query = _context.Competitions
                .Where(c => c.Id == competitionId)
                .Include(c => c.Dances).ThenInclude(d => d.DanceScores)
                .Include(c => c.CompetitionVenuesJoin).ThenInclude(vj => vj.Venue)
                .Include(c => c.CompetitionEntries)
                .Select(q => new CompetitionSummaryDO()
                {
                    Id = q.Id,
                    Name = q.Name,
                    NumberOfCompetitionEntries = q.CompetitionEntries.Where(ce => !ce.IsDeleted && !ce.IsDisabled).Count(),
                    NumberOfTicketsSold = activityQuery.Count(),
                    VenuesCount = q.CompetitionVenuesJoin.Count(),
                    ResultsPublished = q.ResultsPublished,
                    HasBeenPopulated = q.HasBeenPopulated,
                    EntryAttributes = q.CompetitionEntryAttributes,
                    HasDancesGenerated = q.FlowState == CompetitionFlowState.Generated || q.FlowState == CompetitionFlowState.Published || q.Dances.Any(d => d.HasScoresEntered),
                    AllowPopulation = q.InTestingMode || !q.Dances.Any(d => d.HasScoresEntered),
                    AllowDanceGeneration = q.HasBeenPopulated && (q.InTestingMode || !q.Dances.Any(d => d.HasScoresEntered)),
                    AllowAdHocDanceAddition = q.AllowAdHocDanceAddition,
                    CompetitionDanceSummaryDO = new CompetitionDanceSummaryDO
                    {
                        TotalDanceCount = q.Dances.Count(),
                        ResultsEntered = q.Dances.Count(d => d.HasScoresEntered),
                        ResultsChecked = q.Dances.Count(d => d.HasScoresChecked),
                        TotalScoresCount = q.Dances.SelectMany(d => d.DanceScores).Count(),
                        ScoresEntered = q.Dances.Where(d => d.HasScoresEntered).SelectMany(d => d.DanceScores).Count(),
                        ScoresChecked = q.Dances.Where(d => d.HasScoresChecked).SelectMany(d => d.DanceScores).Count(),
                    },
                });


                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<Competition> ClearCompetitionAttendance(int competitionId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Competitions.Where(c => c.Id == competitionId)
                .Include(c => c.CompetitionEntries).ThenInclude(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries);

                var competition = query.FirstOrDefault();

                var entryAttributesToDelete = competition.CompetitionEntries.SelectMany(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries);
                var entriesToDelete = competition.CompetitionEntries;

                _context.DertCompetitionEntryAttributeDertCompetitionEntries.RemoveRange(entryAttributesToDelete);
                _context.CompetitionEntries.RemoveRange(entriesToDelete);

                _context.SaveChanges();

                return competition;

            });

            return await task;
        }
    }
}
