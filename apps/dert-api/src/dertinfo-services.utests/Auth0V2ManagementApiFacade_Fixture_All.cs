using DertInfo.Services.ExternalProviders;
using NSubstitute;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace dertinfo_services.utests
{
    public class Auth0V2ManagementApiFacade_Fixture_All : Auth0V2ManagementApiFacade_Setup
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("token", null)]
        [InlineData(null, "http://someendpoint.url")]
        public void Ensure_That_The_Facade_Fails_If_Not_Configured(string authToken, string managementEndpoint)
        {
            // Arrange 
            this.sut = new Auth0V2ManagementApiFacade();

            // Act & Assert
            Should.Throw<ArgumentException>(() =>
            {
                this.sut.Connect(authToken, new Uri(managementEndpoint));
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Ensure_GetUser_Checks_Arguments(string userId)
        {
            // Arrange
            this.sut = new Auth0V2ManagementApiFacade();
            this.sut.Connect("AUTH_TOKEN", new Uri("http://someendpoint.url"));

            // Act & Asset
            var exceptionType = userId == null ? typeof(ArgumentNullException) : typeof(ArgumentException);
            this.sut.GetUser(userId).ShouldThrow(exceptionType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Ensure_DeleteUser_Checks_Arguments(string userId)
        {
            // Arrange
            this.sut = new Auth0V2ManagementApiFacade();
            this.sut.Connect("AUTH_TOKEN", new Uri("http://someendpoint.url"));

            // Act & Asset
            var exceptionType = userId == null ? typeof(ArgumentNullException) : typeof(ArgumentException);
            this.sut.DeleteUser(userId).ShouldThrow(exceptionType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Ensure_UpdateUser_Checks_Arguments(string userId)
        {
            // Arrange
            this.sut = new Auth0V2ManagementApiFacade();
            this.sut.Connect("AUTH_TOKEN", new Uri("http://someendpoint.url"));
            var userUpdateRequest = new Auth0.ManagementApi.Models.UserUpdateRequest();

            // Act & Asset
            var exceptionType = userId == null ? typeof(ArgumentNullException) : typeof(ArgumentException);
            this.sut.UpdateUser(userId, userUpdateRequest).ShouldThrow(exceptionType);
        }
    }
}
