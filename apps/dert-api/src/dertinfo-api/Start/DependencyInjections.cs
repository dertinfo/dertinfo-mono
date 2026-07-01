using DertInfo.CrossCutting.Auth;
using DertInfo.CrossCutting.Configuration;
using DertInfo.CrossCutting.Connection;
using DertInfo.Repository;
using DertInfo.Services;
using DertInfo.Services.Entity.Activities;
using DertInfo.Services.Entity.CompetitionEntrants;
using DertInfo.Services.Entity.Competitions;
using DertInfo.Services.Entity.CompetitionTemplates;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.DanceScoreParts;
using DertInfo.Services.Entity.DodResults;
using DertInfo.Services.Entity.DodSubmissions;
using DertInfo.Services.Entity.DodTalks;
using DertInfo.Services.Entity.DodUsers;
using DertInfo.Services.Entity.EmailTemplates;
using DertInfo.Services.Entity.Events;
using DertInfo.Services.Entity.EventSettings;
using DertInfo.Services.Entity.Groups;
using DertInfo.Services.Entity.Images;
using DertInfo.Services.Entity.JudgeSlots;
using DertInfo.Services.Entity.MemberAttendance;
using DertInfo.Services.Entity.Notifications;
using DertInfo.Services.Entity.SystemSettings;
using DertInfo.Services.Entity.TeamAttendances;
using DertInfo.Services.ExternalProviders;
using DertInfo.Services.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DertInfo.Api.Start
{
    public static class DependencyInjections
    {
        public static void Apply(IServiceCollection services, IConfiguration configuration)
        {
            //DertInfo
            services.AddSingleton<IStorageAccountConnection>(new StorageAccountConnection(configuration));
            services.AddSingleton<IDertInfoConfiguration>(new DertInfoConfiguration(configuration));

            //services.AddSingleton<IFileUtilities, FileUtilities>();
            services.AddSingleton<IImageUtilities, ImageUtilities>();

            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IAuth0V2ManagementApiClient, Auth0V2ManagementApiClient>();
            services.AddTransient<IAuth0V2ManagementApiFacade, Auth0V2ManagementApiFacade>();
            services.AddTransient<IAuth0V2ManagementApiFactory, Auth0V2ManagementApiFactory>();
            services.AddTransient<IAuth0V2ManagementApiTokenRequester, Auth0V2ManagementApiTokenRequester>();
            services.AddTransient<ITeamAttendanceService, TeamAttendanceService>();
            services.AddTransient<IMemberAttendanceService, MemberAttendanceService>();
            services.AddTransient<ICompetitionAttendanceService, CompetitionAttendanceService>();
            services.AddTransient<ICompetitionService, CompetitionService>();
            services.AddTransient<ICompetitionTemplateService, CompetitionTemplateService>();
            services.AddTransient<IDataObfuscationService, DataObfuscationService>();
            services.AddTransient<IDanceGenerationService, DanceGenerationService>();
            services.AddTransient<IDanceService, DanceService>();
            services.AddTransient<IDanceScorePartsService, DanceScorePartsService>();
            services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            services.AddTransient<IEmailSendingService, EmailSendingServiceMailGun>();
            services.AddTransient<IEventService, EventService>();
            services.AddTransient<IEventSettingService, EventSettingService>();
            services.AddTransient<IEventTemplateService, EventTemplateService>();

            // DodResults
            services.AddTransient<IDodResultCache, DodResultCache>();
            services.AddTransient<IDodResultCollator, DodResultCollator>();
            services.AddTransient<IDodResultCreator, DodResultCreator>();
            services.AddTransient<IDodResultJudgeValidator, DodResultJudgeValidator>();
            services.AddTransient<IDodResultReader, DodResultReader>();
            services.AddTransient<IDodResultService, DodResultService>();
            services.AddTransient<IDodResultUpdater, DodResultUpdater>();

            // DodSubmissions
            services.AddTransient<IDodSubmissionCreator, DodSubmissionCreator>();
            services.AddTransient<IDodSubmissionDeleter, DodSubmissionDeleter>();
            services.AddTransient<IDodSubmissionReader, DodSubmissionReader>();
            services.AddTransient<IDodSubmissionService, DodSubmissionService>();
            services.AddTransient<IDodSubmissionUpdater, DodSubmissionUpdater>();

            // DodUsers
            services.AddTransient<IDodUserBlocker, DodUserBlocker>();
            services.AddTransient<IDodUserCreator, DodUserCreator>();
            services.AddTransient<IDodUserReader, DodUserReader>();
            services.AddTransient<IDodUserService, DodUserService>();
            services.AddTransient<IDodUserUpdater, DodUserUpdater>();

            // DodUsers
            services.AddTransient<IDodTalkService, DodTalkService>();

            // Group
            services.AddTransient<IGroupCreator, GroupCreator>();
            services.AddTransient<IGroupDeleter, GroupDeleter>();
            services.AddTransient<IGroupObfuscator, GroupObfuscator>();
            services.AddTransient<IGroupReader, GroupReader>();
            services.AddTransient<IGroupUpdater, GroupUpdater>();
            services.AddTransient<IGroupService, GroupService>();

            services.AddTransient<IGroupMemberService, GroupMemberService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IInvoiceService, InvoiceService>();
            services.AddTransient<IJudgeSlotService, JudgeSlotService>();
            services.AddTransient<IPaperworkDataService, PaperworkDataService>();
            services.AddTransient<IPricingService, PricingService>();
            services.AddTransient<IRegistrationEmailDataService, RegistrationEmailDataService>();
            services.AddTransient<IRegistrationService, RegistrationService>();
            services.AddTransient<IResultByActivityCache, ResultByActivityCache>();
            services.AddTransient<IResultByActivityService, ResultByActivityService>();

            // Notifications
            services.AddTransient<INotificationChecker, NotificationChecker>();
            services.AddTransient<INotificationCreator, NotificationCreator>();
            services.AddTransient<INotificationDeleter, NotificationDeleter>();
            services.AddTransient<INotificationReader, NotificationReader>();
            services.AddTransient<INotificationUpdater, NotificationUpdater>();
            services.AddTransient<INotificationService, NotificationService>();

            // System Settings
            services.AddTransient<ISystemSettingReader, SystemSettingReader>();
            services.AddTransient<ISystemSettingUpdater, SystemSettingUpdater>();
            services.AddTransient<ISystemSettingService, SystemSettingService>();

            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IVenueService, VenueService>();

            // services.AddTransient<IAttendanceClassificationRepository, AttendanceClassificationRepository>();
            services.AddTransient<IActivityRepository, ActivityRepository>();
            services.AddTransient<IActivityMemberAttendanceRepository, ActivityMemberAttendanceRepository>();
            services.AddTransient<IActivityTeamAttendanceRepository, ActivityTeamAttendanceRepository>();
            services.AddTransient<IBlobStorageRepository, BlobStorageRepository>();
            services.AddTransient<ICacheRepository, CacheRepository>();
            services.AddTransient<ICompetitionRepository, CompetitionRepository>();
            services.AddTransient<ICompetitionEntryRepository, CompetitionEntryRepository>();
            services.AddTransient<ICompetitionEntryAttributeRepository, CompetitionEntryAttributeRepository>();
            services.AddTransient<ICompetitionScoreCategoryRepository, CompetitionScoreCategoryRepository>();
            services.AddTransient<ICompetitionScoreSetRepository, CompetitionScoreSetRepository>();
            services.AddTransient<IDanceRepository, DanceRepository>();
            services.AddTransient<IDanceScorePartRepository, DanceScorePartRepository>();
            services.AddTransient<IDodResultRepository, DodResultRepository>();
            services.AddTransient<IDodResultComplaintRepository, DodResultComplaintRepository>();
            services.AddTransient<IDodSubmissionRepository, DodSubmissionRepository>();
            services.AddTransient<IDodTalkRepository, DodTalkRepository>();
            services.AddTransient<IDodUserRepository, DodUserRepository>();
            services.AddTransient<IEmailTemplateRepository, EmailTemplateRepository>();
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IEventSettingRepository, EventSettingRepository>();
            services.AddTransient<IGroupMemberRepository, GroupMemberRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<IJudgeRepository, JudgeRepository>();
            services.AddTransient<IJudgeSlotRepository, JudgeSlotRepository>();
            services.AddTransient<IMarkingSheetRepository, MarkingSheetRepository>();
            services.AddTransient<IMemberAttendanceRepository, MemberAttendanceRepository>();
            services.AddTransient<INotificationAudienceLogRepository, NotificationAudienceLogRepository>();
            services.AddTransient<INotificationLastCheckRepository, NotificationLastCheckRepository>();
            services.AddTransient<INotificationMessageRepository, NotificationMessageRepository>();
            services.AddTransient<IRegistrationRepository, RegistrationRepository>();
            services.AddTransient<ISystemSettingRepository, SystemSettingRepository>();
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<ITeamAttendanceRepository, TeamAttendanceRepository>();
            services.AddTransient<IVenueRepository, VenueRepository>();

            services.AddScoped<IDertInfoUser, DertInfoUser>();

            
        }
    }
}
