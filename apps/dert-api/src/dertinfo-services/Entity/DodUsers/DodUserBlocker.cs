using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodUsers
{
    public interface IDodUserBlocker
    {
        Task<bool> SetBlockedStatus(int userId, bool setBlocked);
    }

    public class DodUserBlocker: IDodUserBlocker
    {

        IDodUserRepository _dodUserRepository;
        IDodResultRepository _dodResultRepository;

        public DodUserBlocker(
            IDodUserRepository dodUserRepository,
            IDodResultRepository dodResultRepository
        )
        {
            _dodUserRepository = dodUserRepository;
            _dodResultRepository = dodResultRepository;
        }

        /// <summary>
        /// Sets the user account to blocked and removes visibility of all results
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> SetBlockedStatus(int userId, bool setBlocked)
        {
            var user = await this._dodUserRepository.GetByIdWithResults(userId);

            user.ResultsBlocked = setBlocked;
            foreach (var result in user.DodResults) { result.IncludeInScores = !setBlocked; }

            return await this._dodUserRepository.Update(user);
        }
    }
}
