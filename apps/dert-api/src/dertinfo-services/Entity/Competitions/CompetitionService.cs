using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.DomainObjects.Structures;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using DertInfo.Services.Entity.CompetitionEntrants;
using DertInfo.Services.Entity.Dances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Competitions
{
    public interface ICompetitionService
    {
        Task<Competition> FindById(int competitionId);
        Task<IEnumerable<Competition>> GetResultsLookupInfo();
        Task<IEnumerable<EventCompetitionDO>> ListByEvent(int eventId);
        Task<Competition> GetForAuthorization(int competitionId);
        Task<CompetitionSummaryDO> GetForSummary(int competitionId);
        Task<Competition> GetForSettings(int competitionId);
        Task<Competition> UpdateCompetitionSettings(Competition myCompetition);
        Task<IEnumerable<ScoreSet>> GetScoreSets(int competitionId);
        Task<IEnumerable<ScoreCategory>> GetScoreCategories(int competitionId);
        Task<IEnumerable<Venue>> GetVenues(int competitionId);
        Task<IEnumerable<Judge>> GetJudges(int competitionId);
        Task<IEnumerable<CompetitionEntryAttribute>> GetEntryAttributes(int competitionId);
        Task<IEnumerable<CompetitionTeamEntryDO>> GetEntrants(int competitionId);
        Task<IEnumerable<Dance>> GetDances(int competitionId);
        Task<ScoreSet> UpdateScoreSet(ScoreSet scoreSet);
        Task<ScoreCategory> UpdateScoreCategory(ScoreCategory scoreCategory);
        Task<Venue> UpdateVenue(Venue venue);
        Task<JudgeSlot> UpdateJudgeSlot(JudgeSlot judgeSlot);
        Task<Venue> CreateVenue(int competitionId, Venue venue);
        Task<Judge> CreateJudge(int competitionId, Judge judge);
        Task<Judge> UpdateJudge(Judge judge);
        Task<IEnumerable<Judge>> LookupAllJudges(int competitionId);
        Task<IEnumerable<Judge>> AttachJudges(int competitionId, int[] judgeIds);
        Task<IEnumerable<Judge>> DetachJudges(int competitionId, int[] judgeIds);
        Task<CompetitionEntry> AttachEntryAttribute(int competitionId, int competitionEntryId, int entryAttributeId);
        Task<CompetitionEntry> DetachEntryAttribute(int competitionId, int competitionEntryId, int entryAttributeId);
        Task<IEnumerable<CompetitionEntry>> ClearCompetitionAttendance(int competitionId);
        Task<IEnumerable<Dance>> ClearCompetitionDances(int competitionId);
        Task<IEnumerable<CompetitionEntry>> ResetCompetitionAttendance(int competitionId);
        Task<IEnumerable<Dance>> ResetCompetitionDances(int competitionId);
        Task<IEnumerable<CompetitionEntry>> ApplyCompetitionAttendance(int competitionId);
        Task<IEnumerable<Dance>> ApplyCompetitionDances(int competitionId);
        Task<Dance> AddAdHocDance(int competitionId, int competitionEntryId, int venueId);
        Task<IEnumerable<Venue>> LookupAllVenues(int competitionId);
        Task<IEnumerable<Venue>> AttachVenues(int competitionId, int[] venuesIds);
        Task<IEnumerable<Venue>> DetachVenues(int competitionId, int[] venueIds);
    }

    public class CompetitionService : ICompetitionService
    {
        IActivityRepository _activityRepository;
        IDanceGenerationService _danceGenerationService;
        IDanceService _danceService;
        ICompetitionAttendanceService _competitionAttendanceService;
        ICompetitionEntryRepository _competitionEntryRepository;
        ICompetitionEntryAttributeRepository _competitionEntryAttributeRepository;
        ICompetitionRepository _competitionRepository;
        ICompetitionScoreSetRepository _scoreSetRepository;
        ICompetitionScoreCategoryRepository _scoreCategoryRepository;
        IJudgeRepository _judgeRepository;
        IJudgeSlotRepository _judgeSlotRepository;
        ITeamAttendanceRepository _teamAttendanceRepository;
        IVenueRepository _venueRepository;


        public CompetitionService(
            IActivityRepository activityRepository,
            IDanceGenerationService danceGenerationService,
            IDanceService danceService,
            ICompetitionAttendanceService competitionAttendanceService,
            ICompetitionEntryRepository competitionEntryRepository,
            ICompetitionEntryAttributeRepository competitionEntryAttributeRepository,
            ICompetitionRepository competitionRepository,
            ICompetitionScoreSetRepository scoreSetRepository,
            ICompetitionScoreCategoryRepository scoreCategoryRepository,
            IJudgeRepository judgeRepository,
            IJudgeSlotRepository judgeSlotRepository,
            ITeamAttendanceRepository teamAttendanceRepository,
            IVenueRepository venueRepository
           )
        {
            _activityRepository = activityRepository;
            _danceGenerationService = danceGenerationService;
            _danceService = danceService;
            _competitionAttendanceService = competitionAttendanceService;
            _competitionEntryRepository = competitionEntryRepository;
            _competitionEntryAttributeRepository = competitionEntryAttributeRepository;
            _competitionRepository = competitionRepository;
            _scoreSetRepository = scoreSetRepository;
            _scoreCategoryRepository = scoreCategoryRepository;
            _judgeRepository = judgeRepository;
            _judgeSlotRepository = judgeSlotRepository;
            _teamAttendanceRepository = teamAttendanceRepository;
            _venueRepository = venueRepository;
        }

        public async Task<Competition> GetForAuthorization(int competitionId)
        {
            return await _competitionRepository.GetById(competitionId);
        }

        public async Task<Competition> FindById(int competitionId)
        {
            var competition = await _competitionRepository.GetCompetitionExpandedById(competitionId);
            return competition;
        }

        public async Task<IEnumerable<Competition>> GetResultsLookupInfo()
        {
            var competition = await _competitionRepository.GetAllForResultsLookup();
            return competition;
        }

        public async Task<IEnumerable<EventCompetitionDO>> ListByEvent(int eventId)
        {
            var competitionActivities = await _activityRepository.GetByEventWhereIsCompetition(eventId);

            var competitionIds = competitionActivities.Select(ca => (int)ca.CompetitionId).Distinct();
            var competitions = await _competitionRepository.GetMany(competitionIds);

            return this.BuildEventCompetitionResponses(competitionActivities, competitions);
        }

        public Task<CompetitionSummaryDO> GetForSummary(int competitionId)
        {
            return _competitionRepository.GetForSummary(competitionId);
        }

        public Task<Competition> GetForSettings(int competitionId)
        {
            return _competitionRepository.GetById(competitionId);
        }

        public async Task<Competition> UpdateCompetitionSettings(Competition updatedCompetition)
        {
            // Prevent system becoming unstable by incongruency between judges per venue and score sets
            var scoreSets = await this.GetScoreSets(updatedCompetition.Id);
            if (updatedCompetition.JudgeRequirementPerVenue % scoreSets.Count() != 0)
            {
                throw new Exception("Number of judges per venue is incongruent with the score sets count");
            }

            var originalCompetition = await this._competitionRepository.GetById(updatedCompetition.Id);
            if (originalCompetition == null) { throw new InvalidOperationException("Competition Could Not Be Found"); }

            if (originalCompetition.JudgeRequirementPerVenue != updatedCompetition.JudgeRequirementPerVenue)
            {
                originalCompetition.JudgeRequirementPerVenue = updatedCompetition.JudgeRequirementPerVenue;
            }

            if (originalCompetition.ResultsPublished != updatedCompetition.ResultsPublished)
            {
                originalCompetition.ResultsPublished = updatedCompetition.ResultsPublished;
            }

            if (originalCompetition.ResultsAreCollated != updatedCompetition.ResultsAreCollated)
            {
                originalCompetition.ResultsAreCollated = updatedCompetition.ResultsAreCollated;
            }

            if (originalCompetition.InTestingMode != updatedCompetition.InTestingMode)
            {
                originalCompetition.InTestingMode = updatedCompetition.InTestingMode;
            }

            if (originalCompetition.AllowAdHocDanceAddition != updatedCompetition.AllowAdHocDanceAddition)
            {
                originalCompetition.AllowAdHocDanceAddition = updatedCompetition.AllowAdHocDanceAddition;
            }

            await _competitionRepository.Update(originalCompetition);

            return originalCompetition;
        }

        public async Task<IEnumerable<ScoreSet>> GetScoreSets(int competitionId)
        {
            return await this._scoreSetRepository.GetScoreSets(competitionId);
        }

        public async Task<IEnumerable<ScoreCategory>> GetScoreCategories(int competitionId)
        {
            return await this._scoreCategoryRepository.Find(sc => sc.CompetitionAppliesToId == competitionId);
        }

        public async Task<IEnumerable<Venue>> GetVenues(int competitionId)
        {
            return await this._venueRepository.GetVenuesByCompetition(competitionId);
        }

        public async Task<IEnumerable<Judge>> GetJudges(int competitionId)
        {
            return await this._judgeRepository.GetJudgesByCompetition(competitionId);
        }

        public async Task<IEnumerable<CompetitionEntryAttribute>> GetEntryAttributes(int competitionId)
        {
            return await this._competitionEntryAttributeRepository.Find(cea => cea.CompetitionAppliesToId == competitionId);
        }

        public async Task<IEnumerable<CompetitionTeamEntryDO>> GetEntrants(int competitionId)
        {
            return await this._teamAttendanceRepository.GetTeamEntriesByCompetition(competitionId);
        }

        public async Task<IEnumerable<Dance>> GetDances(int competitionId)
        {
            return await this._danceService.ListForCompetition(competitionId);
        }


        #region Update Methods

        public async Task<ScoreSet> UpdateScoreSet(ScoreSet scoreSet)
        {
            var originalScoreSet = await this._scoreSetRepository.GetById(scoreSet.Id);
            if (originalScoreSet == null) { throw new InvalidOperationException("ScoreSet Could Not Be Found"); }

            if (originalScoreSet.Name != scoreSet.Name)
            {
                originalScoreSet.Name = scoreSet.Name;
            }

            await _scoreSetRepository.Update(originalScoreSet);

            return originalScoreSet;
        }

        public async Task<ScoreCategory> UpdateScoreCategory(ScoreCategory scoreCategory)
        {
            var originalScoreCategory = await this._scoreCategoryRepository.GetById(scoreCategory.Id);
            if (originalScoreCategory == null) { throw new InvalidOperationException("ScoreCategory Could Not Be Found"); }

            if (originalScoreCategory.Name != scoreCategory.Name)
            {
                originalScoreCategory.Name = scoreCategory.Name;
            }

            if (originalScoreCategory.MaxMarks != scoreCategory.MaxMarks)
            {
                originalScoreCategory.MaxMarks = scoreCategory.MaxMarks;
            }

            if (originalScoreCategory.Tag != scoreCategory.Tag)
            {
                originalScoreCategory.Tag = scoreCategory.Tag;
            }

            if (originalScoreCategory.SortOrder != scoreCategory.SortOrder)
            {
                originalScoreCategory.SortOrder = scoreCategory.SortOrder;
            }

            await _scoreCategoryRepository.Update(originalScoreCategory);

            return originalScoreCategory;
        }

        public async Task<Venue> UpdateVenue(Venue venue)
        {
            var originalVenue = await this._venueRepository.GetById(venue.Id);
            if (originalVenue == null) { throw new InvalidOperationException("Venue Could Not Be Found"); }

            if (originalVenue.Name != venue.Name)
            {
                originalVenue.Name = venue.Name;
            }

            await _venueRepository.Update(originalVenue);

            return originalVenue;
        }

        public async Task<JudgeSlot> UpdateJudgeSlot(JudgeSlot judgeSlot)
        {
            var originalJudgeSlot = await this._judgeSlotRepository.GetById(judgeSlot.Id);
            if (originalJudgeSlot == null) { throw new InvalidOperationException("JudgeSlot Could Not Be Found"); }

            if (originalJudgeSlot.JudgeId != judgeSlot.JudgeId)
            {
                if (judgeSlot.JudgeId > 0)
                {
                    originalJudgeSlot.JudgeId = judgeSlot.JudgeId;
                }
                else
                {
                    originalJudgeSlot.Judge = null;
                    originalJudgeSlot.JudgeId = null;
                }
            }

            await _judgeSlotRepository.Update(originalJudgeSlot);

            return originalJudgeSlot;
        }

        public async Task<Judge> UpdateJudge(Judge judge)
        {
            var originalJudge = await this._judgeRepository.GetById(judge.Id);
            if (originalJudge == null) { throw new InvalidOperationException("Judge Could Not Be Found"); }

            if (originalJudge.Name != judge.Name)
            {
                originalJudge.Name = judge.Name;
            }

            if (originalJudge.Telephone != judge.Telephone)
            {
                originalJudge.Telephone = judge.Telephone;
            }

            if (originalJudge.Email != judge.Email)
            {
                originalJudge.Email = judge.Email;
            }

            await _judgeRepository.Update(originalJudge);

            return originalJudge;
        }

        #endregion

        #region Create Methods

        public async Task<Venue> CreateVenue(int competitionId, Venue myVenue)
        {
            var competition = await this.FindById(competitionId);
            var scoreSets = await this.GetScoreSets(competitionId);

            if (competition.JudgeRequirementPerVenue % scoreSets.Count() != 0)
            {
                throw new Exception("Cannot attach venue to competition when the number of judges per venue is not divisible by the score sets");
            }

            // Create the join between the venue and the competition it was created for
            myVenue.CompetitionVenuesJoin.Add(new CompetitionVenuesJoin
            {
                CompetitionId = competitionId
            });

            // Create the judge Slots appropraite for the competition
            for (int i = 0; i < competition.JudgeRequirementPerVenue; i++)
            {
                var scoreSetIndex = i % scoreSets.Count();

                try
                {
                    myVenue.JudgeSlots.Add(new JudgeSlot
                    {
                        CompetitionId = competitionId,
                        JudgeId = null,
                        ScoreSet = scoreSets.ToArray()[scoreSetIndex],
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "System",
                        ModifiedBy = "System",
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            myVenue = await _venueRepository.Add(myVenue);

            return myVenue;
        }

        public async Task<Judge> CreateJudge(int competitionId, Judge myJudge)
        {
            var competition = await this.FindById(competitionId);

            // Create the join between the judge and the event it was created for
            myJudge.EventJudges = new List<EventJudge>();
            myJudge.EventJudges.Add(new EventJudge
            {
                EventId = competition.EventId,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                CreatedBy = "System",
                ModifiedBy = "System"
            });

            // Create the join between the judge and the event it was created for
            myJudge.CompetitionJudges = new List<CompetitionJudge>();
            myJudge.CompetitionJudges.Add(new CompetitionJudge
            {
                CompetitionId = competition.Id,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                CreatedBy = "System",
                ModifiedBy = "System"
            });

            myJudge = await _judgeRepository.Add(myJudge);

            return myJudge;
        }





        #endregion

        #region Assosiation Methods

        public async Task<IEnumerable<Judge>> AttachJudges(int competitionId, int[] judgeIds)
        {
            var competition = await this.FindById(competitionId);
            var judges = new List<Judge>();

            foreach (int judgeId in judgeIds)
            {
                var judge = await this._judgeRepository.GetByIdExpandedByAttachments(judgeId);
                judges.Add(judge);

                // Add this Judge to the competition
                bool alreadyJudging = judge.CompetitionJudges.Any(cj => cj.CompetitionId == competitionId);
                if (!alreadyJudging)
                {
                    judge.CompetitionJudges.Add(new CompetitionJudge
                    {
                        CompetitionId = competitionId,
                        JudgeId = judgeId,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "System",
                        ModifiedBy = "System",
                    });
                }

                // Add this Judge to the event if not already added.
                bool attachedToEvent = judge.EventJudges.Any(ej => ej.EventId == competition.EventId);
                if (!attachedToEvent)
                {
                    judge.EventJudges.Add(new EventJudge
                    {
                        EventId = competition.EventId,
                        JudgeId = judgeId,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "System",
                        ModifiedBy = "System",
                    });
                }

                await this._judgeRepository.Update(judge);
            }

            return judges;
        }

        public async Task<IEnumerable<Judge>> DetachJudges(int competitionId, int[] judgeIds)
        {
            var judges = new List<Judge>();

            foreach (int judgeId in judgeIds)
            {
                var judge = await this._judgeRepository.GetByIdExpandedByAttachments(judgeId);
                judges.Add(judge);

                // Remove this Judge to from the competition
                judge.CompetitionJudges = judge.CompetitionJudges.Where(cj => cj.CompetitionId != competitionId).ToList();

                var slotsToDeallocate = judge.JudgeSlots.Where(js => js.CompetitionId == competitionId).ToList();

                foreach (var judgeSlot in slotsToDeallocate)
                {
                    await this._judgeRepository.ClearJudgeAllocationFromSlot(judgeSlot.Id);
                }

                await this._judgeRepository.Update(judge);
            }

            return judges;
        }

        public async Task<CompetitionEntry> AttachEntryAttribute(int competitionId, int competitionEntryId, int competitionEntryAttributeId)
        {
            // Ensure that the attachment is valid
            var validAttributes = await this.GetEntryAttributes(competitionId);
            var validEntrants = await this.GetEntrants(competitionId);

            if (!validAttributes.Any(eAtt => eAtt.Id == competitionEntryAttributeId))
            {
                throw new Exception("The provided attribute Id is not appropriate for the specified competition");
            }

            // note - we need the null check here as there may be an confirmed entrant but they may not have the matching competition entry
            if (!validEntrants.Any(entry => entry.CompetitionEntry != null && entry.CompetitionEntry.Id == competitionEntryId))
            {
                throw new Exception("The provided competitionEntry Id is not appropriate for the specified competition");
            }

            var competitionEntry = await this._competitionEntryRepository.GetById(competitionEntryId);
            var competitionEntryAttribute = await this._competitionEntryAttributeRepository.GetById(competitionEntryAttributeId);

            if (!competitionEntry.DertCompetitionEntryAttributeDertCompetitionEntries.Any(dceadce => dceadce.DertCompetitionEntryAttributeId == competitionEntryAttributeId))
            {
                DertCompetitionEntryAttributeDertCompetitionEntry theJoin = new DertCompetitionEntryAttributeDertCompetitionEntry
                {
                    DertCompetitionEntryId = competitionEntry.Id,
                    DertCompetitionEntry = competitionEntry,
                    DertCompetitionEntryAttributeId = competitionEntryAttributeId,
                    DertCompetitionEntryAttribute = competitionEntryAttribute
                };

                competitionEntry.DertCompetitionEntryAttributeDertCompetitionEntries.Add(theJoin);

                await this._competitionEntryRepository.Update(competitionEntry);
            }

            return competitionEntry;
        }

        public async Task<CompetitionEntry> DetachEntryAttribute(int competitionId, int competitionEntryId, int competitionEntryAttributeId)
        {
            var competitionEntry = await this._competitionEntryRepository.GetByIdExpandedByEntryAttributes(competitionEntryId);

            competitionEntry.DertCompetitionEntryAttributeDertCompetitionEntries = competitionEntry.DertCompetitionEntryAttributeDertCompetitionEntries.Where(dceadce => dceadce.DertCompetitionEntryAttributeId != competitionEntryAttributeId).ToList();

            await this._competitionEntryRepository.Update(competitionEntry);

            return competitionEntry;
        }

        public async Task<IEnumerable<Venue>> AttachVenues(int competitionId, int[] venueIds)
        {
            var competition = await this.FindById(competitionId);
            var scoreSets = await this._scoreSetRepository.Find(ss => ss.CompetitionId == competitionId);

            var venues = new List<Venue>();

            foreach (int venueId in venueIds)
            {
                var venue = await this._venueRepository.GetVenueExpandedByJudgeSlots(venueId);
                venues.Add(venue);

                // Add this Venue to the competition
                bool alreadyAttached = venue.CompetitionVenuesJoin.Any(cvj => cvj.CompetitionId == competitionId);
                if (!alreadyAttached)
                {
                    venue.CompetitionVenuesJoin.Add(new CompetitionVenuesJoin
                    {
                        CompetitionId = competitionId,
                        VenueId = venueId
                    });

                    // Create the judge Slots appropraite for the competition
                    for (int i = 0; i < competition.JudgeRequirementPerVenue; i++)
                    {
                        var scoreSetIndex = i % scoreSets.Count();

                        try
                        {
                            venue.JudgeSlots.Add(new JudgeSlot
                            {
                                CompetitionId = competitionId,
                                JudgeId = null,
                                ScoreSet = scoreSets.ToArray()[scoreSetIndex],
                                DateCreated = DateTime.Now,
                                DateModified = DateTime.Now,
                                CreatedBy = "System",
                                ModifiedBy = "System",
                            });
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    await this._venueRepository.Update(venue);

                    // Before returning restrict the judge slots to the current competition
                    venue.JudgeSlots = venue.JudgeSlots.Where(js => js.CompetitionId == competitionId).ToList();
                }
            }

            return venues;
        }

        public async Task<IEnumerable<Venue>> DetachVenues(int competitionId, int[] venueIds)
        {
            var venues = new List<Venue>();

            foreach (int venueId in venueIds)
            {
                var venue = await this._venueRepository.GetVenueExpandedByJudgeSlots(venueId);
                venues.Add(venue);

                // Remove this Venue from the competition
                venue.CompetitionVenuesJoin = venue.CompetitionVenuesJoin.Where(cvj => cvj.CompetitionId != competitionId).ToList();

                var slotsToDeallocate = venue.JudgeSlots.Where(js => js.CompetitionId == competitionId).ToList();
                foreach (var judgeSlot in slotsToDeallocate)
                {
                    await this._judgeRepository.ClearJudgeAllocationFromSlot(judgeSlot.Id);
                    await this._judgeSlotRepository.Delete(judgeSlot);
                }

                await this._venueRepository.Update(venue);
            }

            return venues;
        }

        #endregion

        #region Processing Methods

        public async Task<IEnumerable<CompetitionEntry>> ClearCompetitionAttendance(int competitionId)
        {
            return await this._competitionAttendanceService.DeleteAllEntriesForCompetition(competitionId);
        }

        public async Task<IEnumerable<Dance>> ClearCompetitionDances(int competitionId)
        {
            return await this._danceGenerationService.DeleteAllDancesForCompetition(competitionId);
        }

        public async Task<IEnumerable<Dance>> ResetCompetitionDances(int competitionId)
        {
            return await this._danceGenerationService.ReGenerateForCompetition(competitionId);
        }

        public async Task<IEnumerable<CompetitionEntry>> ResetCompetitionAttendance(int competitionId)
        {
            return await this._competitionAttendanceService.RePopulateForCompetition(competitionId);
        }

        public async Task<IEnumerable<Dance>> ApplyCompetitionDances(int competitionId)
        {
            return await this._danceGenerationService.GenerateForCompetition(competitionId);
        }

        public async Task<Dance> AddAdHocDance(int competitionId, int competitionEntryId, int venueId)
        {
            return await this._danceGenerationService.AddDanceToCompetition(competitionId, competitionEntryId, venueId);
        }

        public async Task<IEnumerable<CompetitionEntry>> ApplyCompetitionAttendance(int competitionId)
        {
            return await this._competitionAttendanceService.PopulateForCompetition(competitionId);
        }



        #endregion

        #region Lookups

        public async Task<IEnumerable<Judge>> LookupAllJudges(int competitionId)
        {
            return await this._judgeRepository.GetAll();
        }

        public async Task<IEnumerable<Venue>> LookupAllVenues(int competitionId)
        {
            var competition = await this._competitionRepository.GetById(competitionId);

            return await this._venueRepository.Find(v => v.EventId == competition.EventId);
        }

        #endregion

        #region Private

        private async Task<IEnumerable<CompetitionEntry>> MigrateAttendanceToCompetitionEntries(int competitionId)
        {
            var myCompetition = await _competitionRepository.GetById(competitionId);

            if (myCompetition.FlowState != CompetitionFlowState.New)
            {
                throw new InvalidOperationException("Cannot migrate attendance when competition is not new");
            }

            var competitionEntrants = await this.GetEntrants(competitionId);

            var competitionEntries = new List<CompetitionEntry>();
            foreach (var entrant in competitionEntrants)
            {
                competitionEntries.Add(await this._competitionEntryRepository.Add(new CompetitionEntry
                {
                    CompetitionId = competitionId,
                    TeamAttendanceId = entrant.TeamAttendance.Id
                }));
            }

            return competitionEntries;

        }

        private IEnumerable<EventCompetitionDO> BuildEventCompetitionResponses(ICollection<Activity> competitionActivities, IEnumerable<Competition> competitions)
        {
            // todo - we should put some guards in place to ensure that the loaded entries are expanded by the properties we need.
            var eventCompeititons = new List<EventCompetitionDO>();

            foreach (var activity in competitionActivities)
            {
                // Prepare the inforamtion to make decisions
                // From Activity
                var participatingTeamsCount = activity.ParticipatingTeams.Where(pt => (pt.TeamAttendance.Registration.FlowState == RegistrationFlowState.Confirmed || pt.TeamAttendance.Registration.FlowState == RegistrationFlowState.Closed)).Count();

                // From Competition
                var myComp = competitions.First(c => c.Id == activity.CompetitionId);
                var venuesCount = myComp.CompetitionVenuesJoin.Count();
                var entrantsCount = myComp.CompetitionEntries.Where(ce => !ce.IsDisabled).Count();
                var judgeSlotCount = myComp.JudgeSlots.Count();
                var judgesCount = myComp.CompetitionJudges.Count();

                // Venues
                bool hasVenues = venuesCount > 0;
                var venuesDetail = new List<string>();
                if (!hasVenues) { venuesDetail.Add("No venues have been created or assigned"); }


                // Judges
                bool hasJudges = judgesCount > 0;
                bool hasAllJudges = judgesCount == judgeSlotCount && judgeSlotCount > 0;
                bool hasAllJudgeAllocations = myComp.JudgeSlots.All(js => js.JudgeId != null) && judgeSlotCount > 0;
                var judgesDetail = new List<string>();
                if (!hasJudges) { judgesDetail.Add("No judges have been created or assigned"); }
                if (hasJudges && !hasAllJudges) { judgesDetail.Add("There are insufficient judges assigned"); }
                if (hasJudges && !hasAllJudgeAllocations) { judgesDetail.Add("Not all venues have judges assigned"); }

                // Entries
                bool hasEntries = entrantsCount > 0;
                bool hasAllEntries = entrantsCount == participatingTeamsCount && participatingTeamsCount > 0;
                var entriesDetail = new List<string>();
                if (!hasEntries) { entriesDetail.Add("Competition has not been populated from sales"); }
                if (hasEntries && !hasAllEntries) { entriesDetail.Add("There is a mismatch between ticket sales and marked entries"); }

                // Status
                bool isEmpty = !hasVenues && !hasJudges && !hasEntries;
                bool isIncomplete = !hasAllJudges || !hasAllEntries;
                bool isNotAssigned = !hasAllJudgeAllocations;

                StatusBlock venues = new StatusBlock
                {
                    Title = "Venues",
                    Flag = this.IdentifyFlag(new bool[] { hasVenues }),
                    SubText = string.Empty,
                    DetailItems = venuesDetail
                };

                StatusBlock judges = new StatusBlock
                {
                    Title = "Judges",
                    Flag = this.IdentifyFlag(new bool[] { hasJudges, hasAllJudges, hasAllJudgeAllocations }),
                    SubText = string.Empty,
                    DetailItems = judgesDetail
                };

                StatusBlock entrants = new StatusBlock
                {
                    Title = "Entrants",
                    Flag = this.IdentifyFlag(new bool[] { hasEntries, hasAllEntries }),
                    SubText = string.Empty,
                    DetailItems = entriesDetail
                };

                StatusBlock status = new StatusBlock
                {
                    Title = "Status",
                    Flag = this.IdentifyFlag(new bool[] { !isEmpty, !isIncomplete, !isNotAssigned }),
                    SubText = string.Empty,
                    DetailItems = null
                };


                var eventComp = new EventCompetitionDO()
                {
                    CompetitionId = myComp.Id,
                    CompetitionName = myComp.Name,
                    Status = status,
                    Venues = venues,
                    Entrants = entrants,
                    Judges = judges
                };

                eventCompeititons.Add(eventComp);
            }

            return eventCompeititons;
        }

        private Flag IdentifyFlag(IEnumerable<bool> flags)
        {
            var truthCount = flags.Where(f => f).Count();
            if (truthCount == flags.Count()) { return Flag.Ok; }
            if (truthCount != flags.Count() && truthCount > 0) { return Flag.Warn; }
            if (truthCount == 0) { return Flag.Critical; }

            return Flag.Info;
        }

        #endregion
    }
}
