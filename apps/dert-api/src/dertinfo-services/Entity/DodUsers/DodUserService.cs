using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodUsers
{

    public interface IDodUserService
    {
        Task<DodUser> GetUserByGuid(Guid guid);
        Task<DodUser> Create(DodUser dodUser);
        Task<ICollection<DodJudgeInfoDO>> ListJudgesWithCompletionCounts();
        Task<bool> BlockUser(int userId);
        Task<bool> UnBlockUser(int userId);
        Task<bool> ExtendRecovery(DodUser dodUser);
    }

    public class DodUserService : IDodUserService
    {
        IDodUserBlocker _dodUserBlocker;
        IDodUserReader _dodUserReader;
        IDodUserCreator _dodUserCreator;
        IDodUserUpdater _dodUserUpdater;

        public DodUserService(
            IDodUserBlocker dodUserBlocker,
            IDodUserCreator dodUserCreator,
            IDodUserReader dodUserReader,
            IDodUserUpdater dodUserUpdater
            )
        {
            this._dodUserBlocker = dodUserBlocker;
            this._dodUserCreator = dodUserCreator;
            this._dodUserReader = dodUserReader;
            this._dodUserUpdater = dodUserUpdater;
        }

        public async Task<DodUser> GetUserByGuid(Guid guid)
        {
            return await this._dodUserReader.GetByGuid(guid);
        }

        public async Task<DodUser> Create(DodUser dodUser)
        {
            return await this._dodUserCreator.Create(dodUser);
        }

        public async Task<ICollection<DodJudgeInfoDO>> ListJudgesWithCompletionCounts()
        {
            return await this._dodUserReader.GetJudgeUsersWithCompletionCounts();
        }

        public async Task<bool> BlockUser(int userId)
        {
            return await this._dodUserBlocker.SetBlockedStatus(userId, true);
        }

        public async Task<bool> UnBlockUser(int userId)
        {
            return await this._dodUserBlocker.SetBlockedStatus(userId, false);
        }

        public async Task<bool> ExtendRecovery(DodUser dodUser)
        {
            return await this._dodUserUpdater.ExtendRecovery(dodUser);
        }
    }
}
