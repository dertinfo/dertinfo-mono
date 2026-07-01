using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using NSubstitute;
using Auth0.ManagementApi.Models;

namespace DertInfo.Services.UTests
{
    public class Auth0V2ManagementApiClient_Fixture_RemoveClaimsFromUser : Auth0V2ManagementApiClient_Setup
    {
        [Fact]
        public async Task Ensure_That_Removing_A_Group_Claim_Succeeds()
        {
            // Arrange - Envionment
            Auth0.ManagementApi.Models.User initialUser = data.BuildAuth0UserWithClaims(new string[] { "GROUP1", "GROUP2" }, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            Auth0.ManagementApi.Models.User updatedUser = data.BuildAuth0UserWithClaims(new string[] { "GROUP1" }, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());
            this.mockManagementApiFacade.GetUser(Arg.Any<string>()).Returns(initialUser);
            this.mockManagementApiFacade.UpdateUser(Arg.Any<string>(), Arg.Any<UserUpdateRequest>()).Returns(updatedUser);
            this.mockManagementApiFactory.GetClient().Returns(this.mockManagementApiFacade);

            // Arrange - Test
            var claimsToRemove = data.BuildUserAccessClaimsToAddOrRemove(new string[] { "GROUP2" }, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());

            // Act
            var claims = await this.sut.removeClaimsFromUser(claimsToRemove);

            // Assert
            foreach (var groupClaim in claimsToRemove.GroupPermissions)
            {
                claims.GroupPermissions.ShouldNotContain(groupClaim);
            }
        }
    }
}
