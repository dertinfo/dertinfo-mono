using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodUsers
{
    public interface IDodUserUpdater
    {
        Task<bool> ExtendRecovery(DodUser dodUser);
    }

    public class DodUserUpdater : IDodUserUpdater
    {
        IDodUserRepository _dodUserRepository;

        public DodUserUpdater(
            IDodUserRepository dodUserRepository
        )
        {
            _dodUserRepository = dodUserRepository;
        }

        public async Task<bool> ExtendRecovery(DodUser dodUser)
        {
            // note - we've just hardcoded for 3 days extension
            dodUser.RecoveryPermittedUntil = DateTime.Now.AddDays(6);

            return await this._dodUserRepository.Update(dodUser);
        }
    }
}
