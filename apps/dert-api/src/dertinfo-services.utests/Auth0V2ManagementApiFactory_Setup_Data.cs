using DertInfo.CrossCutting.Configuration;
using DertInfo.Services.ExternalProviders;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.UTests
{
    public class Auth0V2ManagementApiFactory_Setup_Data
    {
        public Auth0V2ManagementApiFactory_Setup_Data()
        {
        }

        internal void SetAuth0ClientIdInConfiguration(IDertInfoConfiguration mockDertInfoConfiguration, string clientId)
        {
            mockDertInfoConfiguration.Auth0_ManagementClientId.Returns(clientId);
        }

        internal void SetAuth0AuthDomainInConfiguration(IDertInfoConfiguration mockDertInfoConfiguration, string clientSecret)
        {
            mockDertInfoConfiguration.Auth0_ManagementClientSecret.Returns(clientSecret);
        }

        internal void SetTokenRequesterToReturnAuthCode(IAuth0V2ManagementApiTokenRequester mockTokenRequester)
        {
            var myFakeAuthCode = GenerateRandomAlphanumericString(25);
            mockTokenRequester.GetManagementApiToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(myFakeAuthCode);
        }

        internal void SetTokenRequesterToReturnToBeEmpty(IAuth0V2ManagementApiTokenRequester mockTokenRequester)
        {
            mockTokenRequester.GetManagementApiToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(string.Empty);
        }

        internal void SetAuth0ClientSecretInConfiguration(IDertInfoConfiguration mockDertInfoConfiguration, string domain)
        {
            mockDertInfoConfiguration.Auth0_Domain.Returns(domain);
        }

        /// <summary>
        /// Generates a random alphanumeric string.
        /// </summary>
        /// <param name="length">The desired length of the string</param>
        /// <returns>The string which has been generated</returns>
        private string GenerateRandomAlphanumericString(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}
