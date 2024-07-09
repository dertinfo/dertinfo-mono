using Auth0.ManagementApi.Models;
using DertInfo.Models.System;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DertInfo.Services.ExternalProviders
{
    public interface IAuth0V2ManagementApiClient
    {
        Task<UserAccessClaims> addClaimsToUser(UserAccessClaims userAccessClaims);
        Task<UserAccessClaims> removeClaimsFromUser(UserAccessClaims userAccessClaims);
        Task<UserGdprInformation> addGdprInformationToUser(UserGdprInformation userGdprInformation);
        Task deleteUser(string auth0UserId);
        Task<UserSettings> getUserSettings(string auth0UserId);
        Task<UserSettings> updateUserSettings(UserSettings userSettings);
    }

    public class Auth0V2ManagementApiClient : IAuth0V2ManagementApiClient
    { 
        private bool _connected = false;
        private IAuth0V2ManagementApiFacade _managementClientFacade;
        private readonly IAuth0V2ManagementApiFactory _managementClientFactory;

        public Auth0V2ManagementApiClient(IAuth0V2ManagementApiFactory managementApiFactory)
        {
            this._managementClientFactory = managementApiFactory;
        }

        public async Task<UserAccessClaims> addClaimsToUser(UserAccessClaims userAccessClaims)
        {
            if (!this._connected) { await this.Connect(); }

            Auth0.ManagementApi.Models.User retrievedUser = await getAuth0User(userAccessClaims.Auth0UserId);

            if (retrievedUser == null)
            {
                throw new ArgumentException("UserId provided cannot be found to update claims");
            }


            if (retrievedUser.AppMetadata == null)
            {
                /* There is no app meta data on the auth0 user */
                UserAppMetaData myAppMetaData = buildDefaultAppMetaData();

                // Augment the default with data passed
                myAppMetaData.groupadmin = userAccessClaims.GroupPermissions;
                myAppMetaData.eventadmin = userAccessClaims.EventPermissions;
                myAppMetaData.venueadmin = userAccessClaims.VenuePermissions;
                myAppMetaData.groupmember = userAccessClaims.GroupMemberPermissions;

                dynamic appMetaDataDyn = myAppMetaData;
                retrievedUser.AppMetadata = appMetaDataDyn;
            }
            else
            {
                /* There is already some app meta data on the auth0 user */
                UserAppMetaData myAppMetaData = retrievedUser.AppMetadata.ToObject<UserAppMetaData>();

                myAppMetaData = addToAppMetaData(myAppMetaData, userAccessClaims, null);

                dynamic appMetaDataDyn = myAppMetaData;
                retrievedUser.AppMetadata = appMetaDataDyn;
            }

            Auth0.ManagementApi.Models.User updatedUser = await updateUserAtAuth0(userAccessClaims.Auth0UserId, retrievedUser);

            // Build the response - include permissions already at auth0
            UserAppMetaData updatedAppMetaData = updatedUser.AppMetadata.ToObject<UserAppMetaData>();
            return new UserAccessClaims()
            {
                GroupPermissions = updatedAppMetaData.groupadmin,
                EventPermissions = updatedAppMetaData.eventadmin,
                VenuePermissions = updatedAppMetaData.venueadmin,
                GroupMemberPermissions = updatedAppMetaData.groupmember
            };
            
        }

        public async Task<UserAccessClaims> removeClaimsFromUser(UserAccessClaims userAccessClaims)
        {
            if (!this._connected) { await this.Connect(); }

            Auth0.ManagementApi.Models.User retrievedUser = await getAuth0User(userAccessClaims.Auth0UserId);

            if (retrievedUser == null)
            {
                throw new ArgumentException("UserId provided cannot be found to update claims");
            }

            if (retrievedUser.AppMetadata == null)
            {
                throw new InvalidOperationException("Cannot remove claims from a user that has no app metadata");
            }
            else
            {
                /* There is already some app meta data on the auth0 user */

                UserAppMetaData myAppMetaData = retrievedUser.AppMetadata.ToObject<UserAppMetaData>();

                myAppMetaData = removeFromAppMetaData(myAppMetaData, userAccessClaims);

                dynamic appMetaDataDyn = myAppMetaData;
                retrievedUser.AppMetadata = appMetaDataDyn;
            }

            Auth0.ManagementApi.Models.User updatedUser = await updateUserAtAuth0(userAccessClaims.Auth0UserId, retrievedUser);

            // Build the response - include permissions already at auth0
            UserAppMetaData updatedAppMetaData = updatedUser.AppMetadata.ToObject<UserAppMetaData>();
            return new UserAccessClaims()
            {
                GroupPermissions = updatedAppMetaData.groupadmin,
                EventPermissions = updatedAppMetaData.eventadmin,
                VenuePermissions = updatedAppMetaData.venueadmin,
                GroupMemberPermissions = updatedAppMetaData.groupmember
            };

        }

        public async Task<UserGdprInformation> addGdprInformationToUser(UserGdprInformation userGdprInformation)
        {
            if (!this._connected) { await this.Connect(); }

            Auth0.ManagementApi.Models.User retrievedUser = await getAuth0User(userGdprInformation.Auth0UserId);

            if (retrievedUser == null)
            {
                throw new ArgumentException("UserId provided cannot be found to update claims");
            }

            if (retrievedUser.AppMetadata == null)
            {
                /* There is no app meta data on the auth0 user */

                UserAppMetaData defaultAppMetaData = buildDefaultAppMetaData();

                // Augment the default with data passed
                defaultAppMetaData.gdprConsentGained = userGdprInformation.GdprConsentGained;
                defaultAppMetaData.gdprConsentGainedDate = DateTime.Now;

                dynamic appMetaDataDyn = defaultAppMetaData;
                retrievedUser.AppMetadata = appMetaDataDyn;
            }
            else
            {
                /* There is already some app meta data on the auth0 user */

                UserAppMetaData appMetaData = retrievedUser.AppMetadata.ToObject<UserAppMetaData>();

                appMetaData = addToAppMetaData(appMetaData, null, userGdprInformation);

                dynamic appMetaDataDyn = appMetaData;
                retrievedUser.AppMetadata = appMetaDataDyn;
            }

            Auth0.ManagementApi.Models.User updatedUser = await updateUserAtAuth0(userGdprInformation.Auth0UserId, retrievedUser);

            // Build the response - include information already at auth0
            UserAppMetaData updatedAppMetaData = updatedUser.AppMetadata.ToObject<UserAppMetaData>();
            userGdprInformation.GdprConsentGained = updatedAppMetaData.gdprConsentGained;

            return userGdprInformation;
        }

        public async Task deleteUser(string auth0UserId)
        {
            if (!this._connected) { await this.Connect(); }

            await this._managementClientFacade.DeleteUser(auth0UserId);
        }

        public async Task<UserSettings> getUserSettings(string auth0UserId)
        {
            if (!this._connected) { await this.Connect(); }

            var auth0User = await this.getAuth0User(auth0UserId);
            var auth0UserUserMetaData = auth0User.UserMetadata;
            var auth0UserAppMetaData = auth0User.AppMetadata;

            UserSettings userSettings = new UserSettings();
            userSettings.Auth0UserId = auth0UserId;
            userSettings.FirstName = auth0UserUserMetaData != null && auth0UserUserMetaData.firstname != null ? auth0UserUserMetaData.firstname : string.Empty;
            userSettings.LastName = auth0UserUserMetaData != null && auth0UserUserMetaData.lastname != null ? auth0UserUserMetaData.lastname : string.Empty;
            userSettings.Telephone = auth0UserUserMetaData != null && auth0UserUserMetaData.phone != null ? auth0UserUserMetaData.phone : string.Empty;
            userSettings.Email = auth0User != null && auth0User.Email != null ? auth0User.Email : string.Empty;
            userSettings.Picture = auth0User != null && auth0User.Picture != null ? auth0User.Picture : string.Empty;
            userSettings.GdprConsentGained = auth0UserAppMetaData != null && auth0UserAppMetaData.gdprConsentGained != null ? auth0UserAppMetaData.gdprConsentGained : false;
            userSettings.GdprConsentGainedDate = auth0UserAppMetaData != null && auth0UserAppMetaData.gdprConsentGainedDate != null ? auth0UserAppMetaData.gdprConsentGainedDate : null;

            return userSettings;
        }

        public async Task<UserSettings> updateUserSettings(UserSettings userSettings)
        {
            if (!this._connected) { await this.Connect(); }

            Auth0.ManagementApi.Models.User retrievedUser = await getAuth0User(userSettings.Auth0UserId);

            if (retrievedUser == null)
            {
                throw new ArgumentException("UserId provided cannot be found to update settings");
            }

            if (retrievedUser.UserMetadata == null)
            {
                /* There is no user meta data on the auth0 user */
                UserAppUserData defaultAppUserData = buildDefaultAppUserData();

                // Augment the default with data passed
                defaultAppUserData.firstname = userSettings.FirstName;
                defaultAppUserData.lastname = userSettings.LastName;
                defaultAppUserData.phone = userSettings.Telephone;

                dynamic appUserDataDyn = defaultAppUserData;
                retrievedUser.UserMetadata = appUserDataDyn;
            }
            else
            {
                /*There is already some app meta data on the auth0 user */

                UserAppUserData userMetaData = retrievedUser.UserMetadata.ToObject<UserAppUserData>();

                userMetaData = updateAppUserData(userMetaData, userSettings);

                dynamic userMetaDataDyn = userMetaData;
                retrievedUser.UserMetadata = userMetaDataDyn;

            }

            Auth0.ManagementApi.Models.User updatedUser = await updateUserAtAuth0(userSettings.Auth0UserId, retrievedUser);

            // Build the response - include information already at auth0
            UserAppUserData updatedUserMetaData = updatedUser.UserMetadata.ToObject<UserAppUserData>();
            userSettings.FirstName = updatedUserMetaData.firstname;
            userSettings.LastName = updatedUserMetaData.lastname;
            userSettings.Telephone = updatedUserMetaData.phone;

            UserAppMetaData updatedAppMetaData = updatedUser.AppMetadata.ToObject<UserAppMetaData>();
            userSettings.GdprConsentGained = updatedAppMetaData.gdprConsentGained;
            userSettings.GdprConsentGainedDate = updatedAppMetaData.gdprConsentGainedDate;

            return userSettings;
        }

        private UserAppMetaData buildDefaultAppMetaData()
        {
            UserAppMetaData myAppMetaData = new UserAppMetaData();
            myAppMetaData.isSuperAdmin = false;
            myAppMetaData.gdprConsentGained = false;
            myAppMetaData.gdprConsentGainedDate = null;

            return myAppMetaData;
        }

        private UserAppUserData buildDefaultAppUserData()
        {
            UserAppUserData myUserMetaData = new UserAppUserData();
            myUserMetaData.firstname = string.Empty;
            myUserMetaData.lastname = string.Empty;
            myUserMetaData.phone = string.Empty;

            return myUserMetaData;
        }

        private UserAppUserData updateAppUserData(UserAppUserData myAppUserData, UserSettings userSettings)
        {
            if (userSettings != null)
            {
                myAppUserData.firstname = myAppUserData != null && myAppUserData.firstname != null ? userSettings.FirstName : string.Empty;
                myAppUserData.lastname = myAppUserData != null && myAppUserData.lastname != null ? userSettings.LastName : string.Empty;
                myAppUserData.phone = myAppUserData != null && myAppUserData.phone != null ? userSettings.Telephone : string.Empty;
            }

            return myAppUserData;
        }

        private UserAppMetaData addToAppMetaData(UserAppMetaData myAppMetaData, UserAccessClaims userAccessClaims, UserGdprInformation userGdprInformation)
        {
            Ensure.Any.IsNotNull(myAppMetaData);
            Ensure.Any.IsNotNull(userAccessClaims);

            var groupPermissions = myAppMetaData.groupadmin != null ? new List<string>(myAppMetaData.groupadmin) : new List<string>();

            foreach (var groupUuid in userAccessClaims.GroupPermissions)
            {
                if (!groupPermissions.Contains(groupUuid)) { groupPermissions.Add(groupUuid); }
            }

            var eventPermissions = myAppMetaData.eventadmin != null ? new List<string>(myAppMetaData.eventadmin) : new List<string>();
            foreach (var eventUuid in userAccessClaims.EventPermissions)
            {
                if (!eventPermissions.Contains(eventUuid)) { eventPermissions.Add(eventUuid); }
            }

            var venuePermissions = myAppMetaData.venueadmin != null ? new List<string>(myAppMetaData.venueadmin) : new List<string>();
            foreach (var venueUuid in userAccessClaims.VenuePermissions)
            {
                if (!venuePermissions.Contains(venueUuid)) { venuePermissions.Add(venueUuid); }
            }

            var groupMemberPermissions = myAppMetaData.groupmember != null ? new List<string>(myAppMetaData.groupmember) : new List<string>();
            foreach (var groupMemberUuid in userAccessClaims.GroupMemberPermissions)
            {
                if (!groupMemberPermissions.Contains(groupMemberUuid)) { groupMemberPermissions.Add(groupMemberUuid); }
            }

            var isSuperAdmin = myAppMetaData.isSuperAdmin;

            // Refresh the original object
            myAppMetaData.groupadmin = groupPermissions.ToArray();
            myAppMetaData.eventadmin = eventPermissions.ToArray();
            myAppMetaData.venueadmin = venuePermissions.ToArray();
            myAppMetaData.groupmember = groupMemberPermissions.ToArray();
            myAppMetaData.isSuperAdmin = isSuperAdmin;
            

            if (userGdprInformation != null)
            {
                var gdprConsentGained = myAppMetaData.gdprConsentGained || false;
                var gdprConsentGainedDate = myAppMetaData != null && myAppMetaData.gdprConsentGainedDate != null ? myAppMetaData.gdprConsentGainedDate : null;
                if (userGdprInformation != null && userGdprInformation.GdprConsentGained)
                {
                    gdprConsentGained = userGdprInformation.GdprConsentGained; // always true
                    gdprConsentGainedDate = DateTime.Now;
                }

                // Refresh the original object
                myAppMetaData.gdprConsentGained = gdprConsentGained;
                myAppMetaData.gdprConsentGainedDate = gdprConsentGainedDate;
            }

            return myAppMetaData;
        }

        private UserAppMetaData removeFromAppMetaData(UserAppMetaData myAppMetaData, UserAccessClaims userAccessClaims)
        {
            Ensure.Any.IsNotNull(myAppMetaData);
            Ensure.Any.IsNotNull(userAccessClaims);

            var groupPermissions = myAppMetaData.groupadmin != null ? new List<string>(myAppMetaData.groupadmin) : new List<string>();

            foreach (var groupUuid in userAccessClaims.GroupPermissions)
            {
                if (groupPermissions.Contains(groupUuid)) { groupPermissions.Remove(groupUuid); }
            }

            var eventPermissions = myAppMetaData.eventadmin != null ? new List<string>(myAppMetaData.eventadmin) : new List<string>();
            foreach (var eventUuid in userAccessClaims.EventPermissions)
            {
                if (eventPermissions.Contains(eventUuid)) { eventPermissions.Remove(eventUuid); }
            }

            var venuePermissions = myAppMetaData.venueadmin != null ? new List<string>(myAppMetaData.venueadmin) : new List<string>();
            foreach (var venueUuid in userAccessClaims.VenuePermissions)
            {
                if (venuePermissions.Contains(venueUuid)) { venuePermissions.Remove(venueUuid); }
            }

            var groupMemberPermissions = myAppMetaData.groupmember != null ? new List<string>(myAppMetaData.groupmember) : new List<string>();
            foreach (var groupMemberUuid in userAccessClaims.GroupMemberPermissions)
            {
                if (groupMemberPermissions.Contains(groupMemberUuid)) { groupMemberPermissions.Remove(groupMemberUuid); }
            }

            var isSuperAdmin = myAppMetaData.isSuperAdmin;

            // Refresh the original object
            myAppMetaData.groupadmin = groupPermissions.ToArray();
            myAppMetaData.eventadmin = eventPermissions.ToArray();
            myAppMetaData.venueadmin = venuePermissions.ToArray();
            myAppMetaData.groupmember = groupMemberPermissions.ToArray();
            myAppMetaData.isSuperAdmin = isSuperAdmin;
            
            return myAppMetaData;
        }

        private async Task<Auth0.ManagementApi.Models.User> updateUserAtAuth0(string auth0UserId, Auth0.ManagementApi.Models.User updatedUser)
        {
            try
            {
                UserUpdateRequest updateUserRequest = new UserUpdateRequest();

                updateUserRequest.VerifyEmail = false;
                updateUserRequest.VerifyPhoneNumber = false;
                updateUserRequest.VerifyPassword = false;
                updateUserRequest.UserMetadata = updatedUser.UserMetadata;
                updateUserRequest.AppMetadata = updatedUser.AppMetadata;

                Auth0.ManagementApi.Models.User auth0UpdateResponse = await this._managementClientFacade.UpdateUser(auth0UserId, updateUserRequest);

                return auth0UpdateResponse;
            }
            catch (Exception e)
            {
                throw new Exception("Problem with Updating User at Auth0", e);
            }
        }

        private async Task<User> getAuth0User(string auth0UserId)
        {
            try
            {
                return await this._managementClientFacade.GetUser(auth0UserId);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot retrive user from auth service", e);
            }
        }

        private async Task Connect()
        {
            this._managementClientFacade = await this._managementClientFactory.GetClient();
            this._connected = true;
        }

        
    }
}
