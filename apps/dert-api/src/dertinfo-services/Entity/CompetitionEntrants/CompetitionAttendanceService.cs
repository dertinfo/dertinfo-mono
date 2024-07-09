using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.CompetitionEntrants
{
    public interface ICompetitionAttendanceService
    {
        Task<IEnumerable<CompetitionEntry>> PopulateForCompetition(int competitionId);

        Task<IEnumerable<CompetitionEntry>> RePopulateForCompetition(int competitionId);

        Task<IEnumerable<CompetitionEntry>> DeleteAllEntriesForCompetition(int competitionId);
    }

    public class CompetitionAttendanceService : ICompetitionAttendanceService
    {
        IActivityRepository _activityRepository;
        ICompetitionEntryRepository _competitionEntryRepository;
        ICompetitionRepository _competitionRepository;

        public CompetitionAttendanceService(
            IActivityRepository activityRepository,
            ICompetitionEntryRepository competitionEntryRepository,
            ICompetitionRepository competitionRepository
           )
        {
            _activityRepository = activityRepository;
            _competitionEntryRepository = competitionEntryRepository;
            _competitionRepository = competitionRepository;
        }

        public async Task<IEnumerable<CompetitionEntry>> DeleteAllEntriesForCompetition(int competitionId)
        {
            var competition = await _competitionRepository.GetById(competitionId);

            if (competition.FlowState != CompetitionFlowState.Populated)
            {
                throw new Exception("You cannot delete competition entries unless the competition is in state 'Populated'. Ypu may need to delete dances.");
            }

            var competitionEntries = await this._competitionEntryRepository.Find(ce => ce.CompetitionId == competitionId);

            foreach (var competitionEntry in competitionEntries)
            {
                await this._competitionEntryRepository.Delete(competitionEntry);
            }

            // Move the competition state back to 'New'
            competition.FlowState = CompetitionFlowState.New;
            competition.DatePopulated = null;
            competition.HasBeenPopulated = false;
            await this._competitionRepository.Update(competition);

            return competitionEntries;
        }

        public async Task<IEnumerable<CompetitionEntry>> PopulateForCompetition(int competitionId)
        {
            var competition = await _competitionRepository.GetById(competitionId);

            if (competition.FlowState != CompetitionFlowState.New)
            {
                throw new Exception("You cannot populate a competition unless it is in state 'New'.");
            }

            var competitionActivity = await this._activityRepository.GetActivityForCompetition(competitionId);

            var competitionEntriesToCreate = competitionActivity.ParticipatingTeams.Where(pt => pt.TeamAttendance.Registration.FlowState == RegistrationFlowState.Confirmed);

            List<CompetitionEntry> entriesCreated = new List<CompetitionEntry>();
            foreach (var activityTeamAttendance in competitionEntriesToCreate)
            {
                entriesCreated.Add(await this._competitionEntryRepository.Add(new CompetitionEntry
                {
                    TeamAttendanceId = activityTeamAttendance.TeamAttendance.Id,
                    CompetitionId = competitionId,
                }));
            }

            // Move the competition state back to 'New'
            competition.FlowState = CompetitionFlowState.Populated;
            competition.DatePopulated = DateTime.Now;
            competition.HasBeenPopulated = true;
            await this._competitionRepository.Update(competition);

            return entriesCreated;
        }

        public async Task<IEnumerable<CompetitionEntry>> RePopulateForCompetition(int competitionId)
        {
            await this.DeleteAllEntriesForCompetition(competitionId);
            return await this.PopulateForCompetition(competitionId);
        }
    }
}
