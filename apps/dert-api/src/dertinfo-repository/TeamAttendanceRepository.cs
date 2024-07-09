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
    public interface ITeamAttendanceRepository : IRepository<TeamAttendance, int>
    {
        Task<TeamAttendance> MarkDeleted(int id);
        Task<TeamAttendance> AddOrReinstate(TeamAttendance teamAttendance);
        Task<decimal> SumAttendanceSalesForRegistration(int registrationId);
        Task<IEnumerable<CompetitionTeamEntryDO>> GetTeamEntriesByCompetition(int competitionId);
    }

    public class TeamAttendanceRepository : BaseRepository<TeamAttendance, int, DertInfoContext>, ITeamAttendanceRepository
    {
        public TeamAttendanceRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<TeamAttendance> AddOrReinstate(TeamAttendance teamAttendance)
        {
            var task = Task.Run(async () =>
            {
                TeamAttendance deletedTeamAttendance = _context.TeamAttendances.IgnoreQueryFilters()
                    .Where(ma =>
                       ma.TeamId == teamAttendance.TeamId &&
                       ma.RegistrationId == teamAttendance.RegistrationId &&
                       ma.IsDeleted
                    ).FirstOrDefault();

                if (deletedTeamAttendance != null)
                {
                    deletedTeamAttendance.IsDeleted = false;

                    teamAttendance = deletedTeamAttendance;

                    _context.SaveChanges();
                }
                else
                {
                    teamAttendance = await Add(teamAttendance);
                }

                IQueryable<TeamAttendance> query = _context.TeamAttendances
                .Where(ta => ta.Id == teamAttendance.Id)
                .Include(ta => ta.TeamActivities).ThenInclude(teamActivity => teamActivity.Activity);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<IEnumerable<CompetitionTeamEntryDO>> GetTeamEntriesByCompetition(int competitionId)
        {
            /*
             * note - it is intended that we remove the competition entries table and data as the function on this table is the same as the Activity team attendace
             */
            var task = Task.Run(() =>
            {
                var query = _context.ActivityTeamAttendances
                    .Where(ata => ata.Activity.CompetitionId == competitionId && (ata.TeamAttendance.Registration.FlowState == RegistrationFlowState.Confirmed || ata.TeamAttendance.Registration.FlowState == RegistrationFlowState.Closed))
                    .Include(ata => ata.TeamAttendance).ThenInclude(ta => ta.CompetitionEntries).ThenInclude(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries).ThenInclude(dceadce => dceadce.DertCompetitionEntryAttribute)
                    .Include(ata => ata.TeamAttendance).ThenInclude(ta => ta.Team).ThenInclude(t => t.TeamImages).ThenInclude(ti => ti.Image)
                    .AsEnumerable()
                .Select(ata => new CompetitionTeamEntryDO
                {
                    CompetitionEntry = ata.TeamAttendance.CompetitionEntries.Where(ce => ce.CompetitionId == competitionId).FirstOrDefault(),
                    Team = ata.TeamAttendance.Team,
                    EntryAttributes = ata.TeamAttendance.CompetitionEntries.Where(ce => ce.CompetitionId == competitionId).SelectMany(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries.Select(dceadce => dceadce.DertCompetitionEntryAttribute)),
                    TeamAttendance = ata.TeamAttendance,
                    HasBeenAddedToCompetition = ata.TeamAttendance.CompetitionEntries.Where(ce => ce.CompetitionId == competitionId).FirstOrDefault() != null
                });


                var result = query.ToList();
                return result;
            });

            return await task;
        }

        public async Task<TeamAttendance> MarkDeleted(int teamAttendanceId)
        {

            var task = Task.Run(() =>
            {
                var teamAttendance = _context.TeamAttendances.Find(teamAttendanceId);

                teamAttendance.IsDeleted = true;

                _context.SaveChanges();

                return teamAttendance;
            });

            return await task;
        }

        public async Task<decimal> SumAttendanceSalesForRegistration(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var teamAttendances = _context.TeamAttendances
                .Where(mAtt => mAtt.RegistrationId == registrationId)
                .Include(ma => ma.TeamActivities).ThenInclude(mAct => mAct.Activity);

                var attendances = teamAttendances.IgnoreQueryFilters();
                attendances = attendances.Where(ma => !ma.IsDeleted);

                decimal totalMemberPrice = 0;
                foreach (var memberAttendance in attendances.ToList())
                {
                    if (memberAttendance.TeamActivities != null)
                    {
                        totalMemberPrice += memberAttendance.TeamActivities.Where(ta => !ta.IsDeleted).Sum(mAct => mAct.Activity.Price);
                    }
                }

                return totalMemberPrice;
            });

            return await task;
        }
    }
}