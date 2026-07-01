using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Repository;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.DanceScoreParts;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.UTests
{
    public class JudgeSlotService_Setup_Data
    {
        public void ArrangeMockUserToHaveNoPermission(IDertInfoUser mockUser)
        {
            // Define the root behaviour for the dependencies
            mockUser.AuthId = "A_MOCK_AUTH_ID";
            mockUser.ClaimsCompetitionAdmin.Returns(new List<string>());
            mockUser.ClaimsEventAdmin.Returns(new List<string>());
            mockUser.ClaimsGroupAdmin.Returns(new List<string>());
            mockUser.ClaimsGroupMember.Returns(new List<string>());
            mockUser.ClaimsVenueAdmin.Returns(new List<string>());
        }

        public void ArrangeDanceServiceToReturnDance(IDanceService mockDanceService, Dance dance)
        {
            var input = Arg.Any<int>();
            var output = dance;
            var outputTask = Task.FromResult((Dance)output);
            mockDanceService.FindById(input).Returns(outputTask);
        }

        internal void ArrangeMockUserHaveToHavePermissions(IDertInfoUser mockUser, int[] eventIds, int[] groupIds, int[] competitionIds, int[] venueIds, int[] groupMemberIds)
        {
            string[] stringifiedCompetitionIds = competitionIds != null ? competitionIds.Select(i => i.ToString()).ToArray() : new string[0];
            string[] stringifiedEventIds = eventIds != null ? eventIds.Select(i => i.ToString()).ToArray() : new string[0];
            string[] stringifiedGroupIds = groupIds != null ? groupIds.Select(i => i.ToString()).ToArray() : new string[0];
            string[] stringifiedGroupMemberIds = groupMemberIds != null ? groupMemberIds.Select(i => i.ToString()).ToArray() : new string[0];
            string[] stringifiedVenueIds = venueIds != null ? venueIds.Select(i => i.ToString()).ToArray() : new string[0];

            // Define the root behaviour for the dependencies
            mockUser.AuthId = "A_MOCK_AUTH_ID";
            mockUser.ClaimsCompetitionAdmin.Returns(new List<string>(stringifiedCompetitionIds));
            mockUser.ClaimsEventAdmin.Returns(new List<string>(stringifiedEventIds));
            mockUser.ClaimsGroupAdmin.Returns(new List<string>(stringifiedGroupIds));
            mockUser.ClaimsGroupMember.Returns(new List<string>(stringifiedGroupMemberIds));
            mockUser.ClaimsVenueAdmin.Returns(new List<string>(stringifiedVenueIds));
        }

        public void ArrangeDancePartsServiceToReturn6PartsIn2Groups(IDanceScorePartsService mockDanceScorePartsService)
        {
            var input = Arg.Any<int>();
            var output = new List<DanceScorePartDO> {
                this.DanceScorePartDO_1_1(),
                this.DanceScorePartDO_1_2(),
                this.DanceScorePartDO_1_3(),
                this.DanceScorePartDO_2_1(),
                this.DanceScorePartDO_2_2(),
                this.DanceScorePartDO_2_3()
            };
            var outputTask = Task.FromResult((IEnumerable<DanceScorePartDO>)output);
            mockDanceScorePartsService.FindByDanceId(input).Returns(outputTask);
        }

        public Dance Dance_With_Venue_At_Event(int eventId) {

            return new Dance()
            {
                Venue = new Venue()
                {
                    EventId = eventId
                }
            };
        }

        public DanceScorePartDO DanceScorePartDO_1_1()
        {
            return new DanceScorePartDO()
            {
                DanceScorePartId = 0, // new not saved
                DanceScoreId = 1,
                JudgeSlotId = 1,
                ScoreCategoryTag = "MU",
                ScoreCategoryName = "Music",
                SortOrder = 1,
                JudgeName = "Judge 1 Name",
                ScoreGiven = null,
                IsPartOfScoreSet = true
            };
        }

        public DanceScorePartDO DanceScorePartDO_1_2()
        {
            return new DanceScorePartDO()
            {
                DanceScorePartId = 0, // new not saved
                DanceScoreId = 2,
                JudgeSlotId = 1,
                ScoreCategoryTag = "ST",
                ScoreCategoryName = "Stepping",
                SortOrder = 2,
                JudgeName = "Judge 1 Name",
                ScoreGiven = null,
                IsPartOfScoreSet = true
            };
        }

        public DanceScorePartDO DanceScorePartDO_1_3()
        {
            return new DanceScorePartDO()
            {
                DanceScorePartId = 0, // new not saved
                DanceScoreId = 5,
                JudgeSlotId = 1,
                ScoreCategoryTag = "BZ",
                ScoreCategoryName = "Buzz",
                SortOrder = 5,
                JudgeName = "Judge 1 Name",
                ScoreGiven = null,
                IsPartOfScoreSet = true
            };
        }

        public DanceScorePartDO DanceScorePartDO_2_1()
        {
            return new DanceScorePartDO()
            {
                DanceScorePartId = 0, // new not saved
                DanceScoreId = 1,
                JudgeSlotId = 2,
                ScoreCategoryTag = "MU",
                ScoreCategoryName = "Music",
                SortOrder = 1,
                JudgeName = "Judge 2 Name",
                ScoreGiven = null,
                IsPartOfScoreSet = false
            };
        }

        public DanceScorePartDO DanceScorePartDO_2_2()
        {
            return new DanceScorePartDO()
            {
                DanceScorePartId = 0, // new not saved
                DanceScoreId = 2,
                JudgeSlotId = 2,
                ScoreCategoryTag = "ST",
                ScoreCategoryName = "Stepping",
                SortOrder = 2,
                JudgeName = "Judge 2 Name",
                ScoreGiven = null,
                IsPartOfScoreSet = false
            };
        }

        public DanceScorePartDO DanceScorePartDO_2_3()
        {
            return new DanceScorePartDO()
            {
                DanceScorePartId = 0, // new not saved
                DanceScoreId = 5,
                JudgeSlotId = 2,
                ScoreCategoryTag = "BZ",
                ScoreCategoryName = "Buzz",
                SortOrder = 5,
                JudgeName = "Judge 2 Name",
                ScoreGiven = null,
                IsPartOfScoreSet = true
            };
        }

        
    }
}
