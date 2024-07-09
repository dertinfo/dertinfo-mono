using DertInfo.Services.ExternalProviders;
using NSubstitute;
using Shouldly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DertInfo.Services.UTests
{
    public class Auth0V2ManagementApiFactory_Fixture_GetClient: Auth0V2ManagementApiFactory_Setup
    {
        //ERROR IN TEST IN PIPELINES: NSubstitute.Exceptions.UnexpectedArgumentMatcherException : Argument matchers (Arg.Is, Arg.Any) should only be used in place of member arguments. Do not use in a Returns() statement or anywhere else outside of a member call.
        [Fact]
        public async Task Ensure_That_Valid_Configuration_Returns_ManagementApiClient()
        {
            // Arrange
            data.SetAuth0ClientIdInConfiguration(base.mockDertInfoConfiguration, "CLIENT_ID");
            data.SetAuth0ClientSecretInConfiguration(base.mockDertInfoConfiguration, "CLIENT_SECRET");
            data.SetAuth0AuthDomainInConfiguration(base.mockDertInfoConfiguration, "DOMAIN");

            data.SetTokenRequesterToReturnAuthCode(base.mockTokenRequester);

            // Act 
            base.sut = new Auth0V2ManagementApiFactory(base.mockDertInfoConfiguration, base.mockTokenRequester);
            var managementApiClient = await base.sut.GetClient();

            // Assert
            managementApiClient.ShouldNotBeNull(); // Management Api Recieved
            await base.mockTokenRequester.Received(1).GetManagementApiToken(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()); // Token Requested Once
        }

        //ERROR IN TEST IN PIPELINES: NSubstitute.Exceptions.UnexpectedArgumentMatcherException : Argument matchers (Arg.Is, Arg.Any) should only be used in place of member arguments. Do not use in a Returns() statement or anywhere else outside of a member call.
        [Fact]
        public void Ensure_That_Invalid_Configuration_Throws_Error()
        {
            // Arrange
            data.SetAuth0ClientIdInConfiguration(base.mockDertInfoConfiguration, "CLIENT_ID");
            data.SetAuth0ClientSecretInConfiguration(base.mockDertInfoConfiguration, "CLIENT_SECRET");
            data.SetAuth0AuthDomainInConfiguration(base.mockDertInfoConfiguration, "DOMAIN");

            data.SetTokenRequesterToReturnToBeEmpty(base.mockTokenRequester);

            // Act 
            base.sut = new Auth0V2ManagementApiFactory(base.mockDertInfoConfiguration, base.mockTokenRequester);

            // Assert
            base.sut.GetClient().ShouldThrow("Cannot connect to auth0 management Api if the endpoint and credentials are not provided", typeof(ArgumentException));
        }

        [Fact]
        public void Ensure_That_No_Configuration_Data_Throws_ArgumentException()
        {
            // Act
            base.sut = new Auth0V2ManagementApiFactory(base.mockDertInfoConfiguration, null);

            // Assert
            base.sut.GetClient().ShouldThrow("Cannot connect to auth0 management Api if the endpoint and credentials are not provided", typeof(ArgumentException));
        }
    }
}
