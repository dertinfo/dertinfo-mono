using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.SystemSettings
{
    public interface ISystemSettingReader
    {
        Task<ICollection<SystemSetting>> ListAll();
        Task<SystemSetting> GetByKey(string key);
        Task<ICollection<SystemSetting>> GetByKeys(List<string> keys);
    }

    public class SystemSettingReader : ISystemSettingReader
    {
        ISystemSettingRepository _systemSettingRepository;

        public SystemSettingReader(
            ISystemSettingRepository systemSettingRepository
        )
        {
            _systemSettingRepository = systemSettingRepository;
        }

        public async Task<SystemSetting> GetByKey(string key)
        {
            var findResults = await _systemSettingRepository.Find(ss => ss.Ref == key);

            return findResults.First();
        }

        public async Task<ICollection<SystemSetting>> GetByKeys(List<string> keys)
        {
            var findResults = await _systemSettingRepository.Find(ss => keys.Contains(ss.Ref));

            return findResults;
        }

        public async Task<ICollection<SystemSetting>> ListAll()
        {
            return await _systemSettingRepository.GetAll();
        }
    }
}
