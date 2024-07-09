using DertInfo.CrossCutting.Auth;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IUserService
    {
        Task<UserSettings> GetSettings();
        Task<UserSettings> UpdateCurrentUserSettings(UserSettings myUserSettings);
        Task<UserOverview> GetOverview();
    }

    /// <summary>
    /// This user service is intended as an intermediatry between the controllers and the _authService. 
    /// This service can then be used to migrate users to another provider if required.
    /// </summary>
    public class UserService : IUserService
    {
        private IAuthService _authService;
        private IDertInfoUser _user;

        public UserService(

            IAuthService authService,
            IDertInfoUser user
        )
        {
            this._authService = authService;
            this._user = user;
        }

        public async Task<UserOverview> GetOverview()
        {
            // Nothing in it at current
            var userSettings = await this._authService.GetUserSettings(this._user.AuthId);

            UserOverview userOverview = new UserOverview();
            userOverview.FirstName = userSettings.FirstName;
            userOverview.LastName = userSettings.LastName;
            userOverview.Telephone = userSettings.Telephone;
            userOverview.Email = userSettings.Email;
            userOverview.Picture = userSettings.Picture;
            userOverview.GdprConsentGained = userSettings.GdprConsentGained;
            userOverview.GdprConsentGainedDate = userSettings.GdprConsentGainedDate;

            return userOverview;
        }

        public Task<UserSettings> GetSettings()
        {
            var userSettings = this._authService.GetUserSettings(this._user.AuthId);

            return userSettings;
        }

        public async Task<UserSettings> UpdateCurrentUserSettings(UserSettings myUserSettings)
        {
            if (myUserSettings == null) { throw new InvalidOperationException("Settings are null"); }

            // Set the setting update for the current user
            myUserSettings.Auth0UserId = this._user.AuthId;

            if (myUserSettings.FirstName == null || myUserSettings.FirstName == string.Empty)
            {
                throw new InvalidOperationException("First Name Not Set");
            }

            if (myUserSettings.LastName == null || myUserSettings.LastName == string.Empty)
            {
                throw new InvalidOperationException("Last Name Not Set");
            }

            var updatedSettings = await _authService.UpdateUserSettings(myUserSettings);

            return updatedSettings;
        }
    }
}
