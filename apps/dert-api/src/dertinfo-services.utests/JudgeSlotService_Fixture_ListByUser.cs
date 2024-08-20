using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DertInfo.Services.UTests
{
    public class JudgeSlotService_Fixture_ListByUser : JudgeSlotService_Setup
    {
        [Fact]
        public void When_User_Has_No_Event_Admin_An_Exception_Is_Thrown()
        {
            // Arrange
            data.ArrangeMockUserToHaveNoPermission(this.mockDertInfoUser);

            // Act & Assert
            Should.Throw<Exception>(async () => await this.sut.GetSlotsByDanceId(1));
        }

        [Fact]
        public void When_User_Has_Wrong_Event_Admin_An_Exception_Is_Thrown()
        {
            // Arrange
            var eventIdsForUser = new int[] { 2 };
            var eventIdForDanceVenue = 1;
            var dance = data.Dance_With_Venue_At_Event(eventIdForDanceVenue);

            data.ArrangeMockUserHaveToHavePermissions(this.mockDertInfoUser, eventIdsForUser, null, null, null, null);
            data.ArrangeDanceServiceToReturnDance(this.mockDanceService, dance);

            // Act & Assert
            Should.Throw<Exception>(async () => await this.sut.GetSlotsByDanceId(1));
        }

        [Fact]
        public async Task When_Slot_Info_Requested_DancePartService_Is_Called()
        {
            // Arrange
            var eventId = 1;
            var eventIdsForUser = new int[] { eventId };
            var dance = data.Dance_With_Venue_At_Event(eventId);

            data.ArrangeMockUserHaveToHavePermissions(this.mockDertInfoUser, eventIdsForUser, null, null, null, null);
            data.ArrangeDanceServiceToReturnDance(this.mockDanceService, dance);
            data.ArrangeDancePartsServiceToReturn6PartsIn2Groups(this.mockDanceScorePartsService);

            // Act 
            var result = await this.sut.GetSlotsByDanceId(1);

            // Assert
            result.ToList().Count.ShouldBe(2);

        }


    }
}
