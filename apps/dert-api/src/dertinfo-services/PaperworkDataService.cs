using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject.Paperwork;
using DertInfo.Models.DomainObjects.Paperwork;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IPaperworkDataService
    {
        Task<IEnumerable<ScoreSheetDO>> BuildScoreSheetPopulatedData(int competitionId);

        Task<IEnumerable<ScoreSheetSpareDO>> BuildScoreSheetSparesData(int competitionId);

        Task<IEnumerable<SignInSheetDO>> BuildSignInSheetData(int eventId);
    }

    public class PaperworkDataService : IPaperworkDataService
    {

        ICompetitionRepository _competitionRepository;
        IDanceRepository _danceRepository;
        IEventRepository _eventRepository;
        IRegistrationRepository _registrationRepository;
        IJudgeSlotRepository _judgeSlotRepository;
        ICompetitionScoreSetRepository _scoreSetRepository;


        public PaperworkDataService(
            ICompetitionRepository competitionRepository,
            IDanceRepository danceRepository,
            IEventRepository eventRepository,
            IRegistrationRepository registrationRepository,
            IJudgeSlotRepository judgeSlotRepository,
            ICompetitionScoreSetRepository scoreSetRepository
           )
        {
            _competitionRepository = competitionRepository;
            _danceRepository = danceRepository;
            _eventRepository = eventRepository;
            _registrationRepository = registrationRepository;
            _judgeSlotRepository = judgeSlotRepository;
            _scoreSetRepository = scoreSetRepository;
        }

        public async Task<IEnumerable<ScoreSheetDO>> BuildScoreSheetPopulatedData(int competitionId)
        {
            var allScoreSheets = new List<ScoreSheetDO>();
            var allDances = await this._danceRepository.GetDancesExpandedByCompetitionId(competitionId);
            var myCompetition = await this._competitionRepository.GetById(competitionId);
            var myEvent = await this._eventRepository.GetEventWithImagesById(myCompetition.EventId);


            foreach (var dance in allDances)
            {
                // note - Could build a dictionary for this.
                var judgeSlotsForDanceVenue = dance.Venue.JudgeSlots.Where(js => js.CompetitionId == competitionId); //await this._judgeSlotRepository.Find(js => js.VenueId == dance.VenueId && js.CompetitionId == competitionId);
                var entryAttributes = dance.TeamAttendance.CompetitionEntries.Where(ce => ce.CompetitionId == competitionId).SelectMany(ce => ce.DertCompetitionEntryAttributeDertCompetitionEntries.Select(_ => _.DertCompetitionEntryAttribute));

                foreach (var judgeSlot in judgeSlotsForDanceVenue)
                {
                    // We know there is going to be a paperwork item for this slot.

                    var slot = await this._judgeSlotRepository.GetByIdExpanded(judgeSlot.Id);

                    var scoreSheet = new ScoreSheetDO
                    {
                        Dance = dance,
                        CompetitionEntryAttributes = entryAttributes.ToList(),
                        Event = myEvent,
                        Judge = judgeSlot.Judge,
                        Venue = dance.Venue,
                        Competition = myCompetition,
                        Team = dance.TeamAttendance.Team,
                        ScoreCategories = slot.ScoreSet.ScoreSetScoreCategories.Select(sssc => sssc.ScoreCategory).ToList()
                    };

                    allScoreSheets.Add(scoreSheet);
                }
            }

            return allScoreSheets;
        }

        public async Task<IEnumerable<ScoreSheetSpareDO>> BuildScoreSheetSparesData(int competitionId)
        {
            var allScoreSheets = new List<ScoreSheetSpareDO>();
            var allDances = await this._danceRepository.GetDancesExpandedByCompetitionId(competitionId);
            var myCompetition = await this._competitionRepository.GetById(competitionId);
            var myEvent = await this._eventRepository.GetEventWithImagesById(myCompetition.EventId);
            var myScoreSets = await this._scoreSetRepository.GetScoreSets(competitionId);

            foreach (var scoreSet in myScoreSets)
            {
                var scoreSheet = new ScoreSheetSpareDO
                {
                    Competition = myCompetition,
                    Event = myEvent,
                    ScoreCategories = scoreSet.ScoreSetScoreCategories.Select(sssc => sssc.ScoreCategory).ToList()

                };

                allScoreSheets.Add(scoreSheet);

            }

            return allScoreSheets;
        }

        public async Task<IEnumerable<SignInSheetDO>> BuildSignInSheetData(int eventId)
        {
            var myEvent = await this._eventRepository.GetEventWithImagesById(eventId);
            var myRegistrations = await this._registrationRepository.GetForSignInSheets(eventId);

            List<SignInSheetDO> signInSheetsDOs = new List<SignInSheetDO>();
            foreach (var registration in myRegistrations)
            {
                SignInSheetDO signInSheetDO = new SignInSheetDO();
                signInSheetDO.Group = registration.Group;
                signInSheetDO.Event = myEvent;
                signInSheetDO.Registration = registration;
                signInSheetDO.MemberAttendances = registration.MemberAttendances.ToList();
                signInSheetDO.TeamAttendances = registration.TeamAttendances.ToList();

                signInSheetsDOs.Add(signInSheetDO);
            }

            return signInSheetsDOs;
        }
    }
}
