using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodUsers
{
    public interface IDodUserReader
    {
        Task<DodUser> GetByGuid(Guid guid);
        Task<ICollection<DodJudgeInfoDO>> GetJudgeUsersWithCompletionCounts();
    }

    public class DodUserReader : IDodUserReader
    {
        IDodUserRepository _dodUserRepository;
        IDodSubmissionRepository _dodSubmissionRepository;

        public DodUserReader(
            IDodUserRepository dodUserRepository,
            IDodSubmissionRepository dodSubmissionRepository
        ) {
            _dodUserRepository = dodUserRepository;
            _dodSubmissionRepository = dodSubmissionRepository;
        }
        public async Task<DodUser> GetByGuid(Guid guid)
        {
            return await this._dodUserRepository.GetByGuid(guid);
        }

        public async Task<ICollection<DodJudgeInfoDO>> GetJudgeUsersWithCompletionCounts()
        {
            var dodUsers = await this._dodUserRepository.GetUsersWithResults();

            var totalSubmissionCount = await this._dodSubmissionRepository.Count();

            var judgeInfos = new List<DodJudgeInfoDO>();

            foreach(var dodUser in dodUsers) 
            {
                var dodJudgeInfo = new DodJudgeInfoDO();
                dodJudgeInfo.DodUserId = dodUser.Id;
                dodJudgeInfo.JudgeName = dodUser.Name;
                dodJudgeInfo.JudgeEmail = dodUser.Email;
                dodJudgeInfo.ResultsBlocked = dodUser.ResultsBlocked;
                dodJudgeInfo.CountCompleted = dodUser.DodResults.Count;
                dodJudgeInfo.CountToComplete = totalSubmissionCount;
                dodJudgeInfo.IsOfficial = dodUser.IsOfficial;
                dodJudgeInfo.InterestedInJudging = dodUser.InterestedInJudging;

                judgeInfos.Add(dodJudgeInfo);
            }

            return judgeInfos;
        }
    }
}
