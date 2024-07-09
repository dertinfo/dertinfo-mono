using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.SystemSettings
{
    public interface ISystemSettingUpdater
    {
        Task<SystemSetting> UpdateSetting(SystemSetting mySetting);

        Task<SystemSetting> UpdateByKeyAndValue(string key, string value);
    }

    public class SystemSettingUpdater : ISystemSettingUpdater
    {
        ISystemSettingRepository _systemSettingRepository;

        public SystemSettingUpdater(
            ISystemSettingRepository systemSettingRepository
        )
        {
            _systemSettingRepository = systemSettingRepository;
        }

        public async Task<SystemSetting> UpdateByKeyAndValue(string key, string value)
        {
            var foundSettings = await _systemSettingRepository.Find(ss => ss.Ref == key);

            var originalSetting = foundSettings.FirstOrDefault();

            if (originalSetting == null) { throw new InvalidOperationException("System Setting Could Not Be Found"); }

            if (originalSetting.Value != value)
            {
                originalSetting.Value = value;
            }

            await _systemSettingRepository.Update(originalSetting);

            return originalSetting;
        }

        public async Task<SystemSetting> UpdateSetting(SystemSetting updatedSetting)
        {
            var originalSetting = await _systemSettingRepository.GetById(updatedSetting.Id);

            if (originalSetting == null) { throw new InvalidOperationException("System Setting Could Not Be Found"); }

            if (originalSetting.Name != updatedSetting.Name)
            {
                originalSetting.Name = updatedSetting.Name;
            }

            if (originalSetting.Ref != updatedSetting.Ref)
            {
                originalSetting.Ref = updatedSetting.Ref;
            }

            if (originalSetting.Value != updatedSetting.Value)
            {
                originalSetting.Value = updatedSetting.Value;
            }

            await _systemSettingRepository.Update(originalSetting);

            return originalSetting;
        }
    }
}
