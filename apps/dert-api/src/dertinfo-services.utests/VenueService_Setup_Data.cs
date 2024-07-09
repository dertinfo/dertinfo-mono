using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.UTests
{
    public class VenueService_Setup_Data
    {
        private readonly string _emptyAuthToken = string.Empty;
        private readonly string _mockAuthId = "A_MOCK_AUTH_ID";
        private readonly string _venueNamePrefix = "Venue";

        public void ArrangeMockUserToHaveNoPermission(IDertInfoUser mockUser)
        {
            // Define the root behaviour for the dependencies
            mockUser.AuthId = _mockAuthId;
            mockUser.ClaimsCompetitionAdmin.Returns(new List<string>());
            mockUser.ClaimsEventAdmin.Returns(new List<string>());
            mockUser.ClaimsGroupAdmin.Returns(new List<string>());
            mockUser.ClaimsGroupMember.Returns(new List<string>());
            mockUser.ClaimsVenueAdmin.Returns(new List<string>());
        }

        public void ArrangeMockRepoFindToReturn3Items(IVenueRepository mockVenueRepo)
        {
            var input = Arg.Any<Expression<Func<Venue, bool>>>();
            var output = new List<Venue> { SimpleVenue(1), SimpleVenue(2), SimpleVenue(3) };
            var outputTask = Task.FromResult((IList<Venue>)output);
            mockVenueRepo.Find(input).Returns(outputTask);
        }

        public void ArrangeMockUserToHaveEmptyAuthToken(IDertInfoUser mockUser)
        {
            mockUser.AuthId.Returns(_emptyAuthToken);
        }

        private Venue SimpleVenue(int id)
        {
            return new Venue
            {
                Id = id,
                Name = $"{this._venueNamePrefix} {id}",
            };
        }
    }
}
