using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DertInfo.Services.UTests
{
    public class VenueService_Fixture_ListByUser: VenueService_Setup
    {
        //ERROR IN TEST IN PIPELINES:  NSubstitute.Exceptions.UnexpectedArgumentMatcherException
        [Fact]
        public void When_User_Has_No_AuthToken_An_Exception_Is_Thrown()
        {
            // Arrange
            data.ArrangeMockUserToHaveEmptyAuthToken(base.mockDertInfoUser);

            // Assert
            Should.Throw<InvalidOperationException>(async () =>
            {
                await this.sut.ListByUser(1);
            });
        }

        //ERROR IN TEST IN PIPELINES: NSubstitute.Exceptions.AmbiguousArgumentsException : Cannot determine argument specifications to use. Please use specifications for all arguments of the same type.
        [Fact]
        public async Task When_User_Has_No_Permissions_Find_Is_Still_Called()
        {
            // Arrange
            data.ArrangeMockRepoFindToReturn3Items(base.mockVenueRepo);
            data.ArrangeMockUserToHaveNoPermission(base.mockDertInfoUser);

            // Act 
            var result = await this.sut.ListByUser(1);

            // Assert
            result.Count.ShouldBe(3);
        }
    }
}
