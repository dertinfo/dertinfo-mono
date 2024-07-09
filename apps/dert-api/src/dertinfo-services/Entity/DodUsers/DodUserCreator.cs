using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodUsers
{
    public interface IDodUserCreator
    {
        Task<DodUser> Create(DodUser dodUser);
    }

    public class DodUserCreator : IDodUserCreator
    {
        IDodUserRepository _dodUserRepository;

        public DodUserCreator(
            IDodUserRepository dodUserRepository
            )
        {
            _dodUserRepository = dodUserRepository;
        }

        public async Task<DodUser> Create(DodUser dodUser)
        {
            dodUser.RecoveryPermittedUntil = DateTime.Now.AddDays(3);

            return await this._dodUserRepository.Add(dodUser);
        }
    }
}
