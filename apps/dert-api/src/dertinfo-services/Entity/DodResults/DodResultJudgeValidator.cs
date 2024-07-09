using DertInfo.Services.Entity.SystemSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodResults
{
    public interface IDodResultJudgeValidator
    {
        Task<bool> IsOfficalJudgePasswordValid(string password);
    }

    public class DodResultJudgeValidator : IDodResultJudgeValidator
    {
        private const string PASSWORDS_SETTING_KEY = "SYSTEM-DOD-JUDGEPASSWORDS";

        private ISystemSettingService _systemSettingsService;

        public DodResultJudgeValidator(
            ISystemSettingService systemSettingsService
            )
        {
            _systemSettingsService = systemSettingsService;
        }

        public async Task<bool> IsOfficalJudgePasswordValid(string password)
        {
            var setting = await this._systemSettingsService.GetByKey(PASSWORDS_SETTING_KEY);

            var passwords = setting.Value.Split(",");

            return passwords.Any(p => p == password);
        }
    }
}
