using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.DomainObjects;
using DertInfo.Models.DomainObjects.Reports;
using DertInfo.Repository;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.DanceScoreParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.JudgeSlots
{

    public interface IJudgeSlotService
    {
        Task<IEnumerable<JudgeSlotInformationDO>> GetSlotsByDanceId(int danceId);
        Task UpdateJudgeSlotInformationForDance(int danceId, List<JudgeSlotInformationDto> updateJudgeSlotInformation);

        Task<RangeReportDO> GetRangeReport(int competitionId, int judgeId);

    }

    public class JudgeSlotService : IJudgeSlotService
    {
        IDanceService _danceService;
        IDanceScorePartsService _danceScorePartService;
        IJudgeSlotRepository _judgeSlotRepository;
        IDertInfoUser _user;

        public JudgeSlotService(
            IDanceService danceService,
            IDanceScorePartsService danceScorePartService,
            IJudgeSlotRepository judgeSlotRepository, 
            IDertInfoUser user)
        {
            _danceService = danceService;
            _danceScorePartService = danceScorePartService;
            _judgeSlotRepository = judgeSlotRepository;
            _user = user;
        }

        public async Task<IEnumerable<JudgeSlotInformationDO>> GetSlotsByDanceId(int danceId)
        {
            var accessPermitted = false;

            if (_user.AuthId == string.Empty) { throw new InvalidOperationException("JudgeSlotService - GetSlotsByDanceId - No User"); }

            if (_user.ClaimsEventAdmin != null && _user.ClaimsEventAdmin.Count() > 0)
            {
                var dance = await _danceService.FindById(danceId);

                if (_user.ClaimsEventAdmin.Contains(dance.Venue.EventId.ToString()))
                {
                    accessPermitted = true;
                }
            }

            if (!accessPermitted)
            {
                throw new InvalidOperationException("JudgeSlotService - GetSlotsByDanceId - User does not have access to this judgeSlot information.");
            }

            var danceScoreParts = await this._danceScorePartService.FindByDanceId(danceId);

            var grouping = danceScoreParts.GroupBy(k => k.JudgeSlotId);

            var listing = new List<JudgeSlotInformationDO>();
            foreach (var group in grouping)
            {
                var judgeSlotInfo = new JudgeSlotInformationDO();
                judgeSlotInfo.JudgeSlotId = group.Key;
                judgeSlotInfo.JudgeId = group.First().JudgeId;
                judgeSlotInfo.JudgeName = group.First().JudgeName;
                judgeSlotInfo.ScoreParts = new List<DanceScorePartDO>();

                foreach (var danceScorePart in group)
                {
                    judgeSlotInfo.ScoreParts.Add(danceScorePart);
                }

                listing.Add(judgeSlotInfo);
            }

            return listing;
        }

        public async Task UpdateJudgeSlotInformationForDance(int danceId, List<JudgeSlotInformationDto> updateJudgeSlotInformation)
        {
            if (_user.AuthId == string.Empty) { throw new InvalidOperationException("Dance Service - UpdateDanceAndScores - No User"); }

            var dance = await _danceService.FindById(danceId);

            if (dance == null)
            {
                throw new Exception("Dance not found");
            }

            if (!_user.ClaimsEventAdmin.Contains(dance.Venue.EventId.ToString()))
            {
                throw new Exception("User is not venue admin or event admin");
            }

            foreach (var judgeSlotUpdate in updateJudgeSlotInformation)
            {
                if (judgeSlotUpdate.ScoreParts.Count != dance.DanceScores.Count)
                {
                    throw new Exception("Dance score parts passed are invalid");
                }
            }

            foreach (var judgeSlotUpdate in updateJudgeSlotInformation)
            {
                var scoreParts = judgeSlotUpdate.ScoreParts.Select(sp => new DanceScorePartDO()
                {
                    DanceScorePartId = sp.DanceScorePartId,
                    JudgeSlotId = judgeSlotUpdate.JudgeSlotId,
                    DanceScoreId = sp.DanceScoreId,
                    ScoreGiven = sp.ScoreGiven
                });

                await this._danceScorePartService.AddOrUpdateScoreParts(judgeSlotUpdate.JudgeSlotId, scoreParts);

            }
        }

        public async Task<RangeReportDO> GetRangeReport(int competitionId, int judgeId)
        {
            // Should only ever have a judge attached to 1 slot within a competition.
            var judgeSlot = await _judgeSlotRepository.Find(js => js.JudgeId == judgeId && js.CompetitionId == competitionId);

            return await this._judgeSlotRepository.GetJudgeSlotExpandedWithScorePartsById(judgeSlot.First().Id);
        }
    }
}
