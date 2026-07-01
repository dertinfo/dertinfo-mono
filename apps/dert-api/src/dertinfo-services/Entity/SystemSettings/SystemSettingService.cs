using DertInfo.Models.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.SystemSettings
{
    public interface ISystemSettingService
    {
        Task<ICollection<SystemSetting>> ListAll();
        Task<SystemSetting> UpdateSetting(SystemSetting SystemSetting);
        Task<SystemSetting> UpdateSettingByKeyAndValue(string key, string value);
        Task<SystemSetting> GetByKey(string key);
        Task<ICollection<SystemSetting>> GetByKeys(List<string> key);
    }

    public class SystemSettingService : ISystemSettingService
    {

        ISystemSettingReader _systemSettingReader;
        ISystemSettingUpdater _systemSettingUpdater;

        public SystemSettingService(
            ISystemSettingReader SystemSettingReader,
            ISystemSettingUpdater SystemSettingUpdater
            )
        {
            this._systemSettingReader = SystemSettingReader;
            this._systemSettingUpdater = SystemSettingUpdater;
        }

        public async Task<SystemSetting> GetByKey(string key)
        {
            return await this._systemSettingReader.GetByKey(key);
        }

        public async Task<ICollection<SystemSetting>> ListAll()
        {
            return await this._systemSettingReader.ListAll();
        }

        public async Task<ICollection<SystemSetting>> GetByKeys(List<string> keys)
        {
            return await this._systemSettingReader.GetByKeys(keys);
        }

        public async Task<SystemSetting> UpdateSetting(SystemSetting systemSetting)
        {
            return await this._systemSettingUpdater.UpdateSetting(systemSetting);
        }

        public async Task<SystemSetting> UpdateSettingByKeyAndValue(string key, string value)
        {
            return await this._systemSettingUpdater.UpdateByKeyAndValue(key, value);
        }
    }
}
