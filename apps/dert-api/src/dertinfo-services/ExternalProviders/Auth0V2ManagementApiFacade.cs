using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.ExternalProviders
{
    public interface IAuth0V2ManagementApiFacade
    {
        Task<Auth0.ManagementApi.Models.User> GetUser(string auth0UserId);

        Task<Auth0.ManagementApi.Models.User> UpdateUser(string auth0UserId, UserUpdateRequest updateRequest);

        Task DeleteUser(string auth0UserId);
    }

    public class Auth0V2ManagementApiFacade: IAuth0V2ManagementApiFacade
    {
        private ManagementApiClient _auth0ManagementClient;

        public Auth0V2ManagementApiFacade() {
        }

        public void Connect(string authToken, Uri managementEndpoint)
        {
            Ensure.String.IsNotNullOrEmpty(authToken);
            Ensure.String.IsNotNullOrEmpty(managementEndpoint.ToString());

            this._auth0ManagementClient = new ManagementApiClient(authToken, managementEndpoint);
        }

        public async Task<User> GetUser(string auth0UserId)
        {
            Ensure.String.IsNotNullOrEmpty(auth0UserId);
            Ensure.Any.IsNotNull(_auth0ManagementClient);

            return await _auth0ManagementClient.Users.GetAsync(auth0UserId);
        }

        public async Task DeleteUser(string auth0UserId)
        {
            Ensure.String.IsNotNullOrEmpty(auth0UserId);
            Ensure.Any.IsNotNull(_auth0ManagementClient);

            await _auth0ManagementClient.Users.DeleteAsync(auth0UserId);
        }

        public async Task<User> UpdateUser(string auth0UserId, UserUpdateRequest updateRequest)
        {
            Ensure.String.IsNotNullOrEmpty(auth0UserId);
            Ensure.Any.IsNotNull(updateRequest);
            Ensure.Any.IsNotNull(_auth0ManagementClient);

            return await _auth0ManagementClient.Users.UpdateAsync(auth0UserId, updateRequest);
        }
    }
}
