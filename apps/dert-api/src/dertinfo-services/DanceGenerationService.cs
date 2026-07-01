using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using DertInfo.Services.Entity.EventSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IDanceGenerationService
    {
        Task<IEnumerable<Dance>> GenerateForCompetition(int competitionId);

        Task<IEnumerable<Dance>> ReGenerateForCompetition(int competitionId);

        Task<IEnumerable<Dance>> DeleteAllDancesForCompetition(int competitionId);
        Task<Dance> AddDanceToCompetition(int competitionId, int competitionEntryId, int venueId);
    }

    public class DanceGenerationService : IDanceGenerationService
    {
        ICompetitionEntryRepository _competitionEntryRepository;
        ICompetitionRepository _competitionRepository;
        ICompetitionScoreCategoryRepository _scoreCategoryRepository;
        IDanceRepository _danceRepository;
        IEventSettingService _eventSettingsService;
        ITeamRepository _teamRepository;
        ITeamAttendanceRepository _teamAttendanceRepository;
        IVenueRepository _venueRepository;

        public DanceGenerationService(
            ICompetitionEntryRepository competitionEntryRepository,
            ICompetitionRepository competitionRepository,
            ICompetitionScoreCategoryRepository scoreCategoryRepository,
            IDanceRepository danceRepository,
            IEventSettingService eventSettingsService,
            ITeamAttendanceRepository teamAttendanceRepository,
            ITeamRepository teamRepository,
            IVenueRepository venueRepository

            )
        {
            _competitionEntryRepository = competitionEntryRepository;
            _competitionRepository = competitionRepository;
            _scoreCategoryRepository = scoreCategoryRepository;
            _danceRepository = danceRepository;
            _eventSettingsService = eventSettingsService;
            _teamAttendanceRepository = teamAttendanceRepository;
            _teamRepository = teamRepository;
            _venueRepository = venueRepository;
        }

        public async Task<IEnumerable<Dance>> GenerateForCompetition(int competitionId)
        {
            var competition = await _competitionRepository.GetById(competitionId);

            if (competition.FlowState != CompetitionFlowState.Populated)
            {
                throw new Exception("Dance generation is not permitted when the competition is not in state 'Populated'");
            }

            var competitionEntries = await this._competitionEntryRepository.GetByCompetitionId(competitionId);
            var scoreCategories = await this._scoreCategoryRepository.Find(sc => sc.CompetitionAppliesToId == competitionId);
            var venues = await this._venueRepository.GetVenuesByCompetition(competitionId);

            List<Dance> dancesGenerated = new List<Dance>();
            foreach (CompetitionEntry ce in competitionEntries.Where(ce => !ce.IsDisabled).ToList())
            {
                foreach (var venue in venues)
                {
                    //Create a dance stub
                    Dance dance = new Dance();
                    dance.DertYear = competition.EventId;
                    dance.TeamAttendanceId = ce.TeamAttendanceId;
                    dance.VenueId = venue.Id;
                    dance.CompetitionId = competitionId;
                    dance.DanceScores = new List<DanceScore>();

                    await this._danceRepository.Add(dance);
                    dancesGenerated.Add(dance);
                }
            }

            foreach (Dance dance in dancesGenerated)
            {
                foreach (var scoreCategory in scoreCategories)
                {
                    DanceScore danceScore = new DanceScore();
                    danceScore.DanceId = dance.Id;
                    danceScore.ScoreCategoryId = scoreCategory.Id;
                    danceScore.DateCreated = dance.DateCreated;
                    danceScore.DateModified = dance.DateModified;
                    danceScore.CreatedBy = dance.CreatedBy;
                    danceScore.ModifiedBy = dance.ModifiedBy;

                    dance.DanceScores.Add(danceScore);
                }

                await this._danceRepository.Update(dance);
            }

            competition.FlowState = CompetitionFlowState.Generated;
            await this._competitionRepository.Update(competition);

            return dancesGenerated;
        }

        public async Task<IEnumerable<Dance>> ReGenerateForCompetition(int competitionId)
        {
            var competition = await _competitionRepository.GetById(competitionId);

            if (competition.FlowState != CompetitionFlowState.Generated)
            {
                throw new Exception("Dance re generation is not permitted when the competition is not in state 'Generated'");
            }

            await this.DeleteAllDancesForCompetition(competitionId);
            return await this.GenerateForCompetition(competitionId);
        }

        public async Task<IEnumerable<Dance>> DeleteAllDancesForCompetition(int competitionId)
        {
            var competition = await _competitionRepository.GetById(competitionId);

            if (competition.FlowState != CompetitionFlowState.Generated)
            {
                throw new Exception("Dance deletion is not permitted under any other state than 'Generated'");
            }

            var allDances = await _danceRepository.Find(d => d.CompetitionId == competitionId);

            foreach (var dance in allDances)
            {
                await _danceRepository.Delete(dance);
            }

            // Reset the competition to new
            competition.FlowState = CompetitionFlowState.Populated;
            await _competitionRepository.Update(competition);

            return allDances;
        }

        public async Task<Dance> AddDanceToCompetition(int competitionId, int competitionEntryId, int venueId)
        {
            var competition = await _competitionRepository.GetById(competitionId);
            var competitionEntry = await _competitionEntryRepository.GetById(competitionEntryId);
            var scoreCategories = await this._scoreCategoryRepository.Find(sc => sc.CompetitionAppliesToId == competitionId);

            if (competitionEntry.CompetitionId != competition.Id)
            {
                throw new Exception("Cannot Add AdHoc Dance accross Competitions");
            }

            //Create a dance stub
            Dance dance = new Dance();
            dance.DertYear = competition.EventId;
            dance.TeamAttendanceId = competitionEntry.TeamAttendanceId;
            dance.VenueId = venueId;
            dance.CompetitionId = competitionId;
            dance.DanceScores = new List<DanceScore>();

            await this._danceRepository.Add(dance);

            foreach (var scoreCategory in scoreCategories)
            {
                DanceScore danceScore = new DanceScore();
                danceScore.DanceId = dance.Id;
                danceScore.ScoreCategoryId = scoreCategory.Id;
                danceScore.DateCreated = dance.DateCreated;
                danceScore.DateModified = dance.DateModified;
                danceScore.CreatedBy = dance.CreatedBy;
                danceScore.ModifiedBy = dance.ModifiedBy;

                dance.DanceScores.Add(danceScore);
            }

            await this._danceRepository.Update(dance);

            return await this._danceRepository.GetDanceExpandedById(dance.Id);
        }
    }
}
