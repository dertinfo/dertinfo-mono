using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Repository;
using DertInfo.Services.Entity.Competitions;
using DertInfo.Services.Entity.Dances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DanceScoreParts
{
    public interface IDanceScorePartsService
    {
        Task<IEnumerable<DanceScorePartDO>> FindByDanceId(int danceId);

        Task AddOrUpdateScoreParts(int judgeSlotId, IEnumerable<DanceScorePartDO> scoreParts);
    }

    public class DanceScorePartsService : IDanceScorePartsService
    {
        ICompetitionService _competitionService;
        IDanceService _danceService;
        IDanceScorePartRepository _danceScorePartRepository;
        IVenueService _venueService;

        public DanceScorePartsService(
            ICompetitionService competitionService,
            IDanceService danceService,
            IDanceScorePartRepository danceScorePartRepository,
            IVenueService venueService
            )
        {
            _competitionService = competitionService;
            _danceService = danceService;
            _danceScorePartRepository = danceScorePartRepository;
            _venueService = venueService;
        }



        public async Task<IEnumerable<DanceScorePartDO>> FindByDanceId(int danceId)
        {
            var danceScoreParts = await this._danceScorePartRepository.GetAllByDance(danceId);

            if (danceScoreParts.Count > 0)
            {
                return danceScoreParts;
            }

            return await BuildPartsCollection(danceId);

        }

        public async Task AddOrUpdateScoreParts(int judgeSlotId, IEnumerable<DanceScorePartDO> scoreParts)
        {
            // Look to see if any of the score parts has a non 0 ScorePartId
            var hasPreviousValues = scoreParts.Any(sp => sp.DanceScorePartId > 0);

            if (!hasPreviousValues)
            {
                foreach (var scorePart in scoreParts)
                {
                    var dbObj = new DanceScorePart()
                    {
                        JudgeSlotId = judgeSlotId,
                        ScoreGiven = scorePart.ScoreGiven,
                        DanceScoreId = scorePart.DanceScoreId,
                        Id = 0
                    };

                    var updated = await this._danceScorePartRepository.Add(dbObj);
                }
            }
            else
            {
                var currentParts = await _danceScorePartRepository.Find(sp => sp.JudgeSlotId == judgeSlotId);

                foreach (var scorePart in scoreParts)
                {
                    var currentPart = currentParts.First(cp => cp.Id == scorePart.DanceScorePartId);

                    if (currentPart != null)
                    {
                        currentPart.ScoreGiven = scorePart.ScoreGiven;
                        await this._danceScorePartRepository.Update(currentPart);
                    }
                }
            }
        }


        private async Task<IEnumerable<DanceScorePartDO>> BuildPartsCollection(int danceId)
        {
            // These both contain the expanded properties of judgeslots / judges and dancescores
            var dance = await this._danceService.FindById(danceId);
            var venues = await this._competitionService.GetVenues(dance.CompetitionId);


            var danceScorePartsList = new List<DanceScorePartDO>();
            foreach (var judgeSlot in venues.Where(v => v.Id == dance.VenueId).First().JudgeSlots)
            {
                foreach (var danceScore in dance.DanceScores)
                {
                    var danceScorePart = new DanceScorePartDO()
                    {
                        DanceScorePartId = 0, // new not saved
                        DanceScoreId = danceScore.Id,
                        JudgeSlotId = judgeSlot.Id,
                        ScoreCategoryTag = danceScore.ScoreCategory.Tag,
                        ScoreCategoryName = danceScore.ScoreCategory.Name,
                        SortOrder = danceScore.ScoreCategory.SortOrder,
                        JudgeId = judgeSlot.Judge != null ? judgeSlot.Judge.Id : (int?)null,
                        JudgeName = judgeSlot.Judge != null ? judgeSlot.Judge.Name : string.Empty,
                        ScoreGiven = null,
                        IsPartOfScoreSet = danceScore.ScoreCategory.ScoreSetScoreCategories.Any(sssc => sssc.ScoreSetId == judgeSlot.ScoreSetId)
                    };

                    danceScorePartsList.Add(danceScorePart);
                }
            }

            return danceScorePartsList;
        }


    }
}
