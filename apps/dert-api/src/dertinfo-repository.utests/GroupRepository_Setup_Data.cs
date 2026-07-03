using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;

namespace DertInfo.Repository.UTests
{
    public class GroupRepository_Setup_Data
    {
        public static Group MinimalValidGroup() => new Group
        {
            AccessToken = "test-access-token",
            GroupName = "Test Group",
            GroupBio = "Test bio",
            GroupImageUrl = "https://example.test/image.png",
            GroupMemberJoiningPinCode = "1234",
            OriginTown = "Test Town",
            OriginPostcode = "TE1 1ST",
            PrimaryContactName = "Test Contact",
            PrimaryContactNumber = "01234567890",
            PrimaryContactEmail = "test@example.test",
            TermsAndConditionsAgreedBy = "test@example.test",
            GroupVisibilityType = GroupVisibilityType._private,
            TermsAndConditionsAgreed = true,
        };
    }
}
