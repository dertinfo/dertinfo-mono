using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models;
using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;

namespace DertInfo.Api.Core
{
    public class AutoMapperProfileConfiguration : Profile
    {
        private string DetermineEventImagePrimary(string imagesStorageAccountUri, ICollection<EventImage> myEventImages)
        {
            if (myEventImages != null)
            {
                var primary = myEventImages.FirstOrDefault(ei => ei.IsPrimary);
                return primary != null ? $"{imagesStorageAccountUri}/{primary.Image.Container}/{primary.Image.BlobName}" : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        private string DetermineGroupImagePrimary(string imagesStorageAccountUri, ICollection<GroupImage> myGroupImages)
        {
            if (myGroupImages != null)
            {
                var primary = myGroupImages.FirstOrDefault(ei => ei.IsPrimary);
                return primary != null ? $"{imagesStorageAccountUri}/{primary.Image.Container}/{primary.Image.BlobName}" : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        private string DetermineTeamImagePrimary(string imagesStorageAccountUri, Team myTeam)
        {
            if (myTeam != null && myTeam.TeamImages != null)
            {
                var primary = myTeam.TeamImages.FirstOrDefault(ei => ei.IsPrimary);
                return primary != null ? $"{imagesStorageAccountUri}/{primary.Image.Container}/{primary.Image.BlobName}" : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public AutoMapperProfileConfiguration(string imagesStorageAccountUri)
        {
            #region Domain Object To Dto Mappings

            CreateMap<Models.DomainObjects.ActivityAttendanceDO, Models.DataTransferObject.ActivityAttendanceDto>();

            CreateMap<Models.DomainObjects.ContactInfoDO, Models.DataTransferObject.ContactInfoDto>();

            CreateMap<Models.DomainObjects.CompetitionSummaryDO, Models.DataTransferObject.CompetitionSummaryDto>()
                .ForMember(dest => dest.CompetitionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.NumberOfCompetitionEntries, opt => opt.MapFrom(src => src.NumberOfCompetitionEntries))
                .ForMember(dest => dest.NumberOfTicketsSold, opt => opt.MapFrom(src => src.NumberOfTicketsSold))
                .ForMember(dest => dest.ResultsPublished, opt => opt.MapFrom(src => src.ResultsPublished))
                .ForMember(dest => dest.VenuesCount, opt => opt.MapFrom(src => src.VenuesCount))
                .ForMember(dest => dest.HasBeenPopulated, opt => opt.MapFrom(src => src.HasBeenPopulated))
                .ForMember(dest => dest.HasDancesGenerated, opt => opt.MapFrom(src => src.HasDancesGenerated))
                .ForMember(dest => dest.CompetitionDanceSummaryDto, opt => opt.MapFrom(src => src.CompetitionDanceSummaryDO));

            CreateMap<Models.DomainObjects.CompetitionDanceSummaryDO, Models.DataTransferObject.CompetitionDanceSummaryDto>()
                .ForMember(dest => dest.DancesCount, opt => opt.MapFrom(src => src.TotalDanceCount))
                .ForMember(dest => dest.DancesWithScoresTaken, opt => opt.MapFrom(src => src.ResultsEntered))
                .ForMember(dest => dest.DancesWithScoresChecked, opt => opt.MapFrom(src => src.ResultsChecked))
                .ForMember(dest => dest.IndividualScoresCount, opt => opt.MapFrom(src => src.TotalScoresCount))
                .ForMember(dest => dest.IndividualScoresTaken, opt => opt.MapFrom(src => src.ScoresEntered))
                .ForMember(dest => dest.IndividualScoresChecked, opt => opt.MapFrom(src => src.ScoresChecked));

            CreateMap<Models.DomainObjects.CompetitionTeamEntryDO, Models.DataTransferObject.GroupTeamCompetitionEntryDto>()
                 .ForMember(dest => dest.CompetitionEntryId, opt => opt.MapFrom(src => src.CompetitionEntry.Id))
                 .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.Team.Id))
                 .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.Team.GroupId))
                 .ForMember(dest => dest.TeamBio, opt => opt.MapFrom(src => src.Team.TeamBio))
                 .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.TeamName))
                 .ForMember(dest => dest.ShowShowcase, opt => opt.MapFrom(src => src.Team.ShowShowcase))
                 .ForMember(dest => dest.TeamPictureUrl, opt => opt.MapFrom(src => DetermineTeamImagePrimary(imagesStorageAccountUri, src.Team)))
                 .ForMember(dest => dest.EntryAttributes, opt => opt.MapFrom(src => src.EntryAttributes));

            CreateMap<Models.DomainObjects.DanceScorePartDO, Models.DataTransferObject.DanceScorePartDto>();

            CreateMap<Models.DomainObjects.DertOfDerts.DodGroupResultsDO, Models.DataTransferObject.DertOfDerts.DodGroupResultsDto>();
            CreateMap<Models.DomainObjects.DertOfDerts.DodGroupResultsScoreCardDO, Models.DataTransferObject.DertOfDerts.DodGroupResultsScoreCardDto>();
            CreateMap<Models.DomainObjects.DertOfDerts.DodGroupResultsScoreCategoryDO, Models.DataTransferObject.DertOfDerts.DodGroupResultsScoreCategoryDto>();
            CreateMap<Models.DomainObjects.DertOfDerts.DodJudgeInfoDO, Models.DataTransferObject.DertOfDerts.DodJudgeInfoDto>();
            CreateMap<Models.DomainObjects.DertOfDerts.DodResultComplaintDO, Models.DataTransferObject.DertOfDerts.DodResultComplaintDto>();
            CreateMap<Models.DomainObjects.DertOfDerts.DodTeamCollatedResultPairDO, Models.DataTransferObject.DertOfDerts.DodTeamCollatedResultPairDto>();
            CreateMap<Models.DomainObjects.DertOfDerts.DodTeamCollatedResultDO, Models.DataTransferObject.DertOfDerts.DodTeamCollatedResultDto>();
            CreateMap<Models.DomainObjects.DertOfDerts.DodUserResultsDO, Models.DataTransferObject.DertOfDerts.DodUserResultsDto>();


            CreateMap<Models.DomainObjects.EventCompetitionDO, Models.DataTransferObject.EventCompetitionDto>();

            CreateMap<Models.DomainObjects.EventOverviewDO, Models.DataTransferObject.EventDto>();
            CreateMap<Models.DomainObjects.EventOverviewDO, Models.DataTransferObject.EventOverviewDto>()
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.EventImages)));

            CreateMap<Models.DomainObjects.EventInvoiceDO, Models.DataTransferObject.EventInvoiceDto>()
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Registration.Group.GroupName))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.Registration.Group.GroupImages)));

            CreateMap<Models.DomainObjects.GroupOverviewDO, Models.DataTransferObject.GroupDto>()
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.GroupImages)));
            CreateMap<Models.DomainObjects.GroupOverviewDO, Models.DataTransferObject.GroupOverviewDto>()
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.GroupImages)));

            CreateMap<Models.DomainObjects.JudgeSlotInformationDO, Models.DataTransferObject.JudgeSlotInformationDto>();

            CreateMap<Models.DomainObjects.NotificationDetailDO, Models.DataTransferObject.Notifications.NotificationDetailDto>();
            CreateMap<Models.DomainObjects.NotificationSummaryDO, Models.DataTransferObject.Notifications.NotificationSummaryDto>();
            CreateMap<Models.DomainObjects.NotificationThumbnailInfoDO, Models.DataTransferObject.Notifications.NotificationThumbnailInfoDto>();

            CreateMap<Models.DomainObjects.Paperwork.ScoreSheetDO, Models.DataTransferObject.Paperwork.ScoreSheetDto>()
                .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competition.Name))
                .ForMember(dest => dest.DanceId, opt => opt.MapFrom(src => src.Dance.Id))
                .ForMember(dest => dest.JudgeName, opt => opt.MapFrom(src => src.Judge.Name))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.TeamName))
                .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Venue.Name))
                .ForMember(dest => dest.CompetitionEntryAttributes, opt => opt.MapFrom(src => src.CompetitionEntryAttributes))
                .ForMember(dest => dest.ScoreCategories, opt => opt.MapFrom(src => src.ScoreCategories))
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event));

            CreateMap<Models.DomainObjects.Paperwork.ScoreSheetSpareDO, Models.DataTransferObject.Paperwork.ScoreSheetDto>()
                .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competition.Name))
                .ForMember(dest => dest.DanceId, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.JudgeName, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.CompetitionEntryAttributes, opt => opt.MapFrom(src => new List<Models.DataTransferObject.CompetitionEntryAttributeDto>()))
                .ForMember(dest => dest.ScoreCategories, opt => opt.MapFrom(src => src.ScoreCategories))
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event));

            CreateMap<Models.DomainObjects.Paperwork.SignInSheetDO, Models.DataTransferObject.Paperwork.SignInSheetDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.MemberAttendanceCount, opt => opt.MapFrom(src => src.MemberAttendances.Count()))
                .ForMember(dest => dest.TeamAttendanceCount, opt => opt.MapFrom(src => src.TeamAttendances.Count()))
                .ForMember(dest => dest.GroupMemberPinCode, opt => opt.MapFrom(src => src.Group.GroupMemberJoiningPinCode))
                .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event))
                .ForMember(dest => dest.Registration, opt => opt.MapFrom(src => src.Registration))
                .ForMember(dest => dest.MemberAttendances, opt => opt.MapFrom(src => src.MemberAttendances))
                .ForMember(dest => dest.TeamAttendances, opt => opt.MapFrom(src => src.TeamAttendances));

            CreateMap<Models.DomainObjects.Reports.RangeReportDO, Models.DataTransferObject.Reports.RangeReportDto>();
            CreateMap<Models.DomainObjects.Reports.RangeReportSubRangeDO, Models.DataTransferObject.Reports.RangeReportSubRangeDto>();

            CreateMap<Models.DomainObjects.Structures.StatusBlock, Models.DataTransferObject.Structures.StatusBlockDto>();


            CreateMap<Models.System.Results.ScoreGroupResult, Models.DataTransferObject.Results.ScoreGroupResultDto>();
            CreateMap<Models.System.Results.TeamCollatedResult, Models.DataTransferObject.TeamCollatedResultDto>();
            CreateMap<Models.System.Results.TeamCollatedFullResult, Models.DataTransferObject.TeamCollatedFullResultDto>();

            #endregion

            #region System Object To Dto Mapping

            CreateMap<Models.System.UserOverview, Models.DataTransferObject.UserOverviewDto>();
            CreateMap<Models.System.UserAccessClaims, Models.DataTransferObject.UserAccessClaimsDto>();
            CreateMap<Models.System.UserGdprInformation, Models.DataTransferObject.UserGdprInformationDto>();

            #endregion 

            #region Database To Dto Mappings

            CreateMap<Models.Database.Activity, Models.DataTransferObject.ActivityDto>();
            CreateMap<Models.Database.Activity, Models.DataTransferObject.ActivityDetailDto>();

            CreateMap<Models.Database.ActivityMemberAttendance, Models.DataTransferObject.ActivityMemberAttendanceDto>()
                .ForMember(dest => dest.ActivityTitle, opt => opt.MapFrom(src => src.Activity.Title))
                .ForMember(dest => dest.ActivityPrice, opt => opt.MapFrom(src => src.Activity.Price));

            CreateMap<Models.Database.ActivityTeamAttendance, Models.DataTransferObject.ActivityTeamAttendanceDto>()
                .ForMember(dest => dest.ActivityTitle, opt => opt.MapFrom(src => src.Activity.Title))
                .ForMember(dest => dest.ActivityPrice, opt => opt.MapFrom(src => src.Activity.Price));

            CreateMap<Models.Database.Competition, Models.DataTransferObject.EventCompetitionDto>()
                .ForMember(dest => dest.CompetitionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Models.Database.Competition, Models.DataTransferObject.CompetitionSettingsDto>()
                .ForMember(dest => dest.CompetitionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NoOfJudgesPerVenue, opt => opt.MapFrom(src => src.JudgeRequirementPerVenue))
                .ForMember(dest => dest.ResultsPublished, opt => opt.MapFrom(src => src.ResultsPublished))
                .ForMember(dest => dest.ResultsCollated, opt => opt.MapFrom(src => src.ResultsAreCollated));

            CreateMap<Models.Database.CompetitionEntryAttribute, Models.DataTransferObject.CompetitionEntryAttributeDto>();

            //Flatten dance 
            CreateMap<Models.Database.Dance, Models.DataTransferObject.VenueDanceDto>()
                .ForMember(dest => dest.DanceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompetitionId, opt => opt.MapFrom(src => src.CompetitionId))
                .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competition.Name))
                .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamAttendance.Team.Id))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.TeamAttendance.Team.TeamName));

            CreateMap<Models.Database.Dance, Models.DataTransferObject.DanceDetailDto>()
                .ForMember(dest => dest.DanceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competition.Name))
                .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamAttendance.TeamId))
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.TeamAttendance.Team.TeamName))
                .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Venue.Name))
                .ForMember(dest => dest.DanceMarkingSheets, opt => opt.MapFrom(src => src.MarkingSheetImages))
                .ForMember(dest => dest.DanceScores, opt => opt.MapFrom(src => src.DanceScores))
                .ForMember(dest => dest.DateScoresEntered, opt => opt.MapFrom(src => src.DateScoresEntered));

            //[Obsolete] This model is the same as the dance detail dto
            //CreateMap<Models.Database.Dance, Models.DataTransferObject.Results.DanceResultDto>()
            //    .ForMember(dest => dest.DanceId, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.CompetitionName, opt => opt.MapFrom(src => src.Competition.Name))
            //    .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamAttendance.TeamId))
            //    .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.TeamAttendance.Team.TeamName))
            //    .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Venue.Name))
            //    .ForMember(dest => dest.DanceMarkingSheets, opt => opt.MapFrom(src => src.MarkingSheetImages))
            //    .ForMember(dest => dest.DanceScores, opt => opt.MapFrom(src => src.DanceScores));

            CreateMap<Models.Database.DanceScore, Models.DataTransferObject.DanceScoreDto>()
                .ForMember(dest => dest.ScoreCatagoryId, opt => opt.MapFrom(src => src.ScoreCategoryId))
                .ForMember(dest => dest.ScoreCatagoryMaxMarks, opt => opt.MapFrom(src => src.ScoreCategory.MaxMarks))
                .ForMember(dest => dest.ScoreCatagoryName, opt => opt.MapFrom(src => src.ScoreCategory.Name));

            CreateMap<Models.Database.DodResult, Models.DataTransferObject.DertOfDerts.DodResultDto>()
                .ForMember(dest => dest.UserGuid, opt => opt.MapFrom(src => src.DodUser.Guid))
                .ForMember(dest => dest.IsOfficialJudge, opt => opt.MapFrom(src => src.DodUser.IsOfficial));

            CreateMap<Models.Database.DodResultComplaint, Models.DataTransferObject.DertOfDerts.DodResultComplaintDto>();

            CreateMap<Models.Database.DodSubmission, Models.DataTransferObject.DertOfDerts.DodSubmissionDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.HasAnyResults, opt => opt.MapFrom(src => !(src.CumulativeNumberOfResults == 0)))
                .ForMember(dest => dest.NumberOfResults, opt => opt.MapFrom(src => src.CumulativeNumberOfResults))
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.Group.GroupImages)));

            CreateMap<Models.Database.DodTalk, Models.DataTransferObject.DertOfDerts.DodTalkDto>();

            CreateMap<Models.Database.EmailTemplate, Models.DataTransferObject.EmailTemplateDto>();
            CreateMap<Models.Database.EmailTemplate, Models.DataTransferObject.EmailTemplateDetailDto>();

            CreateMap<Models.Emails.GroupRegistationConfirmationEmailData, Models.DataTransferObject.Emails.EmailRegistrationConfirmationDataDto>();
            CreateMap<Models.Emails.TeamAttendanceLineItem, Models.DataTransferObject.Emails.EmailTeamAttendanceLineItemDto>();
            CreateMap<Models.Emails.IndividualAttendanceLineItem, Models.DataTransferObject.Emails.EmailIndividualAttendanceLineItemDto>();
            CreateMap<Models.Emails.ActivityLineItem, Models.DataTransferObject.Emails.EmailActivityLineItemDto>();

            CreateMap<Models.Database.Event, Models.DataTransferObject.EventDto>()
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.EventImages)));

            // [Obsolete 20191004] - Domain object GroupOverviewDO introduced to replace this mapping
            // CreateMap<Models.Database.Event, Models.DataTransferObject.EventOverviewDto>()
            //     .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(src.EventImages)))
            //     .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.EventVisibilityType));

            CreateMap<Models.Database.Event, Models.DataTransferObject.EventShowcaseDto>()
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.EventImages)))
                .ForMember(dest => dest.EventFinished, opt => opt.MapFrom(src => src.EventEndDate < DateTime.Now.AddDays(1)));

            CreateMap<Models.Database.Event, Models.DataTransferObject.EventShowcaseDetailDto>()
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.EventImages)))
                .ForMember(dest => dest.EventFinished, opt => opt.MapFrom(src => src.EventEndDate < DateTime.Now.AddDays(1)));

            CreateMap<Models.Database.Activity, Models.DataTransferObject.EventActivityDto>()
                .ForMember(dest => dest.AttendanceCount, opt => opt.MapFrom(src => src.ParticipatingIndividuals.Count() + src.ParticipatingTeams.Count()))
                .ForMember(dest => dest.ValueOfSales, opt => opt.MapFrom(src => (src.ParticipatingIndividuals.Count() + src.ParticipatingTeams.Count()) * src.Price));

            CreateMap<Models.Database.EventImage, Models.DataTransferObject.EventImageDto>()
                .ForMember(dest => dest.EventImageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.Image.Id))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => $"{imagesStorageAccountUri}/{src.Image.Container}/{src.Image.BlobName}"));

            CreateMap<Models.Database.Group, Models.DataTransferObject.GroupDto>()
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.GroupImages)));

            // [Obsolete 20191004] - Domain object GroupOverviewDO introduced to replace this mapping
            // CreateMap<Models.Database.Group, Models.DataTransferObject.GroupOverviewDto>()
            //    .ForMember(dest => dest.GroupEmail, opt => opt.MapFrom(src => src.PrimaryContactEmail))
            //    .ForMember(dest => dest.ContactTelephone, opt => opt.MapFrom(src => src.PrimaryContactNumber))
            //    .ForMember(dest => dest.ContactName, opt => opt.MapFrom(src => src.PrimaryContactName))
            //    .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(src)))
            //    .ForMember(dest => dest.TeamsCount, opt => opt.MapFrom(src => src.Teams.Count()))
            //    .ForMember(dest => dest.RegistrationsCount, opt => opt.MapFrom(src => src.Registrations.Count()))
            //    .ForMember(dest => dest.MembersCount, opt => opt.MapFrom(src => src.GroupMembers.Count()))
            //    .ForMember(dest => dest.Visibility, opt => opt.MapFrom(src => src.GroupVisibilityType))
            //    .ForMember(dest => dest.UnpaidInvoicesCount, opt => opt.MapFrom(src => src.Registrations.SelectMany(r => r.Invoices).Where(i => !i.HasPaid).Count()));

            CreateMap<Models.Database.GroupImage, Models.DataTransferObject.GroupImageDto>()
                .ForMember(dest => dest.GroupImageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.Image.Id))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => $"{imagesStorageAccountUri}/{src.Image.Container}/{src.Image.BlobName}"));

            CreateMap<Models.Database.GroupMember, Models.DataTransferObject.GroupMemberDto>()
                .ForMember(dest => dest.GroupMemberId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => src.DateJoined != null ? src.DateJoined : src.DateCreated));

            CreateMap<Models.Database.GroupMember, Models.DataTransferObject.GroupMemberDetailDto>()
                .ForMember(dest => dest.GroupMemberId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DateJoined, opt => opt.MapFrom(src => src.DateJoined != null ? src.DateJoined : src.DateCreated));

            CreateMap<Models.Database.Invoice, Models.DataTransferObject.GroupInvoiceDto>()
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Registration.Event.Name))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Registration.Event.EventImages)));

            CreateMap<Models.Database.Invoice, Models.DataTransferObject.EventInvoiceDto>()
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Registration.Group.GroupName))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.Registration.Group.GroupImages)));

            CreateMap<Models.Database.Invoice, Models.DataTransferObject.InvoiceDto>()
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Registration.Event.Name))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Registration.Event.EventImages)));

            CreateMap<Models.Database.Judge, Models.DataTransferObject.JudgeDto>();

            CreateMap<Models.Database.MarkingSheetImage, Models.DataTransferObject.DanceMarkingSheetDto>()
                .ForMember(dest => dest.MarkingSheetId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => $"{imagesStorageAccountUri}/{src.Image.Container}/{src.Image.BlobName}"));

            CreateMap<Models.Database.MemberAttendance, Models.DataTransferObject.MemberAttendanceDto>()
                .ForMember(dest => dest.AttendanceClassificationName, opt => opt.MapFrom(src => src.MemberActivities.Count() > 0 ? src.MemberActivities.First().Activity.Title : "None"))
                .ForMember(dest => dest.AttendanceClassificationPrice, opt => opt.MapFrom(src => src.MemberActivities.Count() > 0 ? src.MemberActivities.First().Activity.Price : 0))
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Registration.Event.EventImages)))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Registration.Event.Name))
                .ForMember(dest => dest.GroupMemberName, opt => opt.MapFrom(src => src.GroupMember.Name))
                .ForMember(dest => dest.GroupMemberType, opt => opt.MapFrom(src => src.GroupMember.MemberType))
                .ForMember(dest => dest.AttendanceActivities, opt => opt.MapFrom(src => src.MemberActivities.OrderBy(ma => ma.ActivityId)));

            CreateMap<Models.Database.NotificationMessage, Models.DataTransferObject.Notifications.NotificationMessageDto>();

            CreateMap<Models.Database.Registration, Models.DataTransferObject.GroupRegistrationDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.Group.GroupImages)))
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Event.EventImages)))
                .ForMember(dest => dest.MemberAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.activeMember).Count()))
                .ForMember(dest => dest.GuestAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.guest).Count()))
                .ForMember(dest => dest.TeamAttendancesCount, opt => opt.MapFrom(src => src.TeamAttendances.Count()));

            CreateMap<Models.Database.Registration, Models.DataTransferObject.GroupRegistrationOverviewDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.Group.GroupImages)))
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Event.EventImages)))
                .ForMember(dest => dest.MemberAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.activeMember).Count()))
                .ForMember(dest => dest.GuestAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.guest).Count()))
                .ForMember(dest => dest.TeamAttendancesCount, opt => opt.MapFrom(src => src.TeamAttendances.Count()))
                .ForMember(dest => dest.IsEventDeleted, opt => opt.MapFrom(src => src.Event.IsDeleted));


            CreateMap<Models.Database.Registration, Models.DataTransferObject.EventRegistrationOverviewDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.Group.GroupImages)))
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Event.EventImages)))
                .ForMember(dest => dest.MemberAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.activeMember).Count()))
                .ForMember(dest => dest.GuestAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.guest).Count()))
                .ForMember(dest => dest.TeamAttendancesCount, opt => opt.MapFrom(src => src.TeamAttendances.Count()))
                .ForMember(dest => dest.IsGroupDeleted, opt => opt.MapFrom(src => src.Group.IsDeleted));

            CreateMap<Models.Database.Registration, Models.DataTransferObject.EventRegistrationDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.Name))
                .ForMember(dest => dest.GroupPictureUrl, opt => opt.MapFrom(src => DetermineGroupImagePrimary(imagesStorageAccountUri, src.Group.GroupImages)))
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Event.EventImages)))
                .ForMember(dest => dest.MemberAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.activeMember).Count()))
                .ForMember(dest => dest.GuestAttendancesCount, opt => opt.MapFrom(src => src.MemberAttendances.Where(ma => ma.GroupMember.MemberType == MemberType.guest).Count()))
                .ForMember(dest => dest.TeamAttendancesCount, opt => opt.MapFrom(src => src.TeamAttendances.Count()));

            // note - only used in sign in sheets
            CreateMap<Models.Database.Registration, Models.DataTransferObject.RegistrationDto>();

            CreateMap<Models.Database.JudgeSlot, Models.DataTransferObject.JudgeSlotDto>()
                .ForMember(dest => dest.JudgeName, opt => opt.MapFrom(src => src.Judge.Name))
                .ForMember(dest => dest.ScoreSetName, opt => opt.MapFrom(src => src.ScoreSet.Name));

            CreateMap<Models.Database.ScoreCategory, Models.DataTransferObject.ScoreCategoryDto>()
                .ForMember(dest => dest.ScoreCategoryId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag))
                .ForMember(dest => dest.MaxMarks, opt => opt.MapFrom(src => src.MaxMarks))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.SortOrder));

            CreateMap<Models.Database.ScoreSet, Models.DataTransferObject.ScoreSetDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CategoryTags, opt => opt.MapFrom(src => src.ScoreSetScoreCategories.Select(sssc => sssc.ScoreCategory.Tag)))
                .ForMember(dest => dest.ScoreSetId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Models.Database.Team, Models.DataTransferObject.GroupTeamDto>()
                 .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.TeamPictureUrl, opt => opt.MapFrom(src => DetermineTeamImagePrimary(imagesStorageAccountUri, src)));

            CreateMap<Models.Database.Team, Models.DataTransferObject.GroupTeamDetailDto>()
                 .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.TeamPictureUrl, opt => opt.MapFrom(src => DetermineTeamImagePrimary(imagesStorageAccountUri, src)));

            CreateMap<Models.Database.Team, Models.DataTransferObject.GroupTeamCompetitionEntryDto>()
                 .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.TeamPictureUrl, opt => opt.MapFrom(src => DetermineTeamImagePrimary(imagesStorageAccountUri, src)));

            CreateMap<Models.Database.TeamAttendance, Models.DataTransferObject.TeamAttendanceDto>()
                .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Registration.Event.Name))
                .ForMember(dest => dest.EventPictureUrl, opt => opt.MapFrom(src => DetermineEventImagePrimary(imagesStorageAccountUri, src.Registration.Event.EventImages)))
                .ForMember(dest => dest.GroupTeamName, opt => opt.MapFrom(src => src.Team.TeamName))
                .ForMember(dest => dest.AttendanceActivities, opt => opt.MapFrom(src => src.TeamActivities));

            CreateMap<Models.Database.TeamImage, Models.DataTransferObject.TeamImageDto>()
                .ForMember(dest => dest.TeamImageId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.Image.Id))
                .ForMember(dest => dest.ImageResourceUri, opt => opt.MapFrom(src => $"{imagesStorageAccountUri}/{src.Image.Container}/{src.Image.BlobName}"));


            CreateMap<Models.Database.SystemSetting, Models.DataTransferObject.SystemSettingDto>();

            CreateMap<Models.Database.Venue, Models.DataTransferObject.VenueDto>();

            CreateMap<Models.Database.Venue, Models.DataTransferObject.VenueAllocationDto>()
                .ForMember(dest => dest.JudgesAllocated, opt => opt.MapFrom(src => !src.JudgeSlots.Any(js => js.JudgeId == null)));

            #endregion 

            #region Dto to System Object Mapping

            CreateMap<Models.DataTransferObject.UserAccessClaimsDto, Models.System.UserAccessClaims>();
            CreateMap<Models.DataTransferObject.UserSettingsDto, Models.System.UserSettings>();
            CreateMap<Models.DataTransferObject.UserOverviewDto, Models.System.UserOverview>();

            #endregion
        }
    }
}
