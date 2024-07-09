
using DertInfo.Services.ExternalProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.DataTransferObject;
using DertInfo.Models.System;

namespace DertInfo.Services
{
    public interface IAuthService
    {
        Task<UserAccessClaims> AddAccessClaims(UserAccessClaims userAccessClaims);
        Task<UserGdprInformation> AddGdprInformation(UserGdprInformation userGdprInformation);
        Task DeleteUserAccount(string authId);
        Task<UserSettings> GetUserSettings(string authId);
        Task<UserAccessClaims> RemoveAccessClaims(UserAccessClaims userAccessClaims);
        Task<UserSettings> UpdateUserSettings(UserSettings originalSettings);
    }
    public class AuthService : IAuthService
    {
        private IAuth0V2ManagementApiClient _auth0ManagementClient;
        
        public AuthService(IAuth0V2ManagementApiClient auth0ManagementClient)
        {
            _auth0ManagementClient = auth0ManagementClient;
        }

        public async Task<UserAccessClaims> AddAccessClaims(UserAccessClaims userAccessClaims)
        {
            UserAccessClaims allUserClaims = await _auth0ManagementClient.addClaimsToUser(userAccessClaims);
            return allUserClaims;
        }

        public async Task<UserAccessClaims> RemoveAccessClaims(UserAccessClaims userAccessClaims)
        {
            UserAccessClaims allUserClaims = await _auth0ManagementClient.removeClaimsFromUser(userAccessClaims);
            return allUserClaims;
        }

        public async Task<UserGdprInformation> AddGdprInformation(UserGdprInformation userGdprInformation)
        {
            UserGdprInformation allUserGdprInformation = await _auth0ManagementClient.addGdprInformationToUser(userGdprInformation);
            return allUserGdprInformation;
        }

        public async Task DeleteUserAccount(string auth0UserId)
        {
            await _auth0ManagementClient.deleteUser(auth0UserId);
        }

        public async Task<UserSettings> GetUserSettings(string auth0UserId)
        {
            return await _auth0ManagementClient.getUserSettings(auth0UserId);
        }

        public async Task<UserSettings> UpdateUserSettings(UserSettings userSettings)
        {
            return await _auth0ManagementClient.updateUserSettings(userSettings);
        }
    }
}
