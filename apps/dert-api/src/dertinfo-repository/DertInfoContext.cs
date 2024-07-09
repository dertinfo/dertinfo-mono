using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using System.Text;

namespace DertInfo.Repository
{
    public partial class DertInfoContext : DbContext
    {
        // Get key and IV from a Base64String or any other ways.
        // You can generate a key and IV using "AesProvider.GenerateKey()"
        private readonly byte[] _encryptionKey = Encoding.ASCII.GetBytes("??`HTV?\ad??\v?>x?");
        private readonly byte[] _encryptionIV = Encoding.ASCII.GetBytes("?mW1%?Q?@k????;\u0001");
        private readonly IEncryptionProvider _provider;

        public DertInfoContext(DbContextOptions<DertInfoContext> options)
            : base(options)
        {
            this._provider = new AesProvider(this._encryptionKey, this._encryptionIV);
        }

        public virtual DbSet<AccessKeyUser> AccessKeyUsers { get; set; }
        public virtual DbSet<AccessKey> AccessKeys { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<ActivityMemberAttendance> ActivityMemberAttendances { get; set; }
        public virtual DbSet<ActivityTeamAttendance> ActivityTeamAttendances { get; set; }
        public virtual DbSet<AttendanceClassification> AttendanceClassifications { get; set; }
        public virtual DbSet<BreadcrumbItem> BreadcrumbItems { get; set; }
        public virtual DbSet<DatabaseCacheItem> DatabaseCache { get; set; }
        public virtual DbSet<CompetitionEntry> CompetitionEntries { get; set; }
        public virtual DbSet<CompetitionEntryAttribute> CompetitionEntryAttributes { get; set; }
        public virtual DbSet<CompetitionVenuesJoin> CompetitionVenuesJoin { get; set; }
        public virtual DbSet<Competition> Competitions { get; set; }
        public virtual DbSet<CompetitionJudge> CompetitionJudges { get; set; }
        public virtual DbSet<DanceScore> DanceScores { get; set; }
        public virtual DbSet<DanceScorePart> DanceScoreParts { get; set; }
        public virtual DbSet<Dance> Dances { get; set; }
        public virtual DbSet<DertCompetitionEntryAttributeDertCompetitionEntry> DertCompetitionEntryAttributeDertCompetitionEntries { get; set; }
        public virtual DbSet<DodResult> DodResult { get; set; }
        public virtual DbSet<DodResultComplaint> DodResultComplaint { get; set; }
        public virtual DbSet<DodSubmission> DodSubmission { get; set; }
        public virtual DbSet<DodUser> DodUser { get; set; }
        public virtual DbSet<DodTalk> DodTalk { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EventImage> EventImages { get; set; }
        public virtual DbSet<EventJudge> EventJudges { get; set; }
        public virtual DbSet<EventSetting> EventSettings { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<GroupImage> GroupImages { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<JudgeSlot> JudgeSlots { get; set; }
        public virtual DbSet<Judge> Judges { get; set; }
        public virtual DbSet<MarkingSheetImage> MarkingSheetImages { get; set; }
        public virtual DbSet<MarkingSheet> MarkingSheets { get; set; }
        public virtual DbSet<MemberAttendance> MemberAttendances { get; set; }
        public virtual DbSet<NavigationItem> NavigationItems { get; set; }
        public virtual DbSet<NotificationAudienceLog> NotificationAudienceLogs { get; set; }
        public virtual DbSet<NotificationLastCheck> NotificationLastChecks { get; set; }
        public virtual DbSet<NotificationMessage> NotificationMessages { get; set; }
        public virtual DbSet<Registration> Registrations { get; set; }
        public virtual DbSet<ScoreCategory> ScoreCategories { get; set; }
        public virtual DbSet<ScoreSetScoreCategory> ScoreSetScoreCategories { get; set; }
        public virtual DbSet<ScoreSet> ScoreSets { get; set; }
        public virtual DbSet<Spectator> Spectators { get; set; }
        public virtual DbSet<StaticResult> StaticResults { get; set; }
        public virtual DbSet<Steward> Stewards { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
        public virtual DbSet<TeamAggregateScore> TeamAggregateScores { get; set; }
        public virtual DbSet<TeamAttendance> TeamAttendances { get; set; }
        public virtual DbSet<TeamImage> TeamImages { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<Venue> Venues { get; set; }
        public virtual DbSet<WebpagesMembership> WebpagesMembership { get; set; }
        public virtual DbSet<WebpagesOauthMembership> WebpagesOauthMembership { get; set; }
        public virtual DbSet<WebpagesRoles> WebpagesRoles { get; set; }
        public virtual DbSet<WebpagesUsersInRoles> WebpagesUsersInRoles { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseEncryption(this._provider);

            modelBuilder.Entity<AccessKeyUser>(entity =>
            {
                entity.HasIndex(e => e.AccessKeyId)
                    .HasName("IX_AccessKeyId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.DeletePermitted).HasDefaultValueSql("0");

                entity.Property(e => e.EditPermitted).HasDefaultValueSql("0");

                entity.Property(e => e.ViewPermitted).HasDefaultValueSql("0");

                entity.HasOne(d => d.AccessKey)
                    .WithMany(p => p.AccessKeyUsers)
                    .HasForeignKey(d => d.AccessKeyId)
                    .HasConstraintName("FK_dbo.AccessKeyUsers_dbo.AccessKeys_AccessKeyId");
            });

            modelBuilder.Entity<AccessKey>(entity =>
            {
                entity.Property(e => e.AccessKeyRef).HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<AttendanceClassification>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.Property(e => e.ClassificationPrice).HasColumnType("decimal");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.IsDefault).HasDefaultValueSql("0");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.AttendanceClassifications)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_dbo.AttendanceClassifications_dbo.Events_EventId");
            });

            modelBuilder.Entity<BreadcrumbItem>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.IsTypeRoot).HasDefaultValueSql("0");

                entity.Property(e => e.Label).HasColumnName("label");

                entity.Property(e => e.LineageIndex).HasColumnName("lineageIndex");
            });

            modelBuilder.Entity<CompetitionEntry>(entity =>
            {
                entity.HasIndex(e => e.CompetitionId)
                    .HasName("IX_CompetitionId");

                entity.HasIndex(e => e.TeamAttendanceId)
                    .HasName("IX_TeamAttendanceId");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'2015-02-18T00:00:00.000'");

                entity.Property(e => e.DateModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'2015-02-18T00:00:00.000'");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.CompetitionEntries)
                    .HasForeignKey(d => d.CompetitionId)
                    .HasConstraintName("FK_dbo.CompetitionEntries_dbo.Competitions_CompetitionId");

                entity.HasOne(d => d.TeamAttendance)
                    .WithMany(p => p.CompetitionEntries)
                    .HasForeignKey(d => d.TeamAttendanceId)
                    .HasConstraintName("FK_dbo.CompetitionEntries_dbo.TeamAttendances_TeamAttendanceId");
            });

            modelBuilder.Entity<CompetitionEntryAttribute>(entity =>
            {
                entity.HasIndex(e => e.CompetitionAppliesToId)
                    .HasName("IX_CompetitionAppliesToId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.CompetitionAppliesTo)
                    .WithMany(p => p.CompetitionEntryAttributes)
                    .HasForeignKey(d => d.CompetitionAppliesToId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dbo.CompetitionEntryAttributes_dbo.Competitions_CompetitionAppliesToId");
            });

            modelBuilder.Entity<CompetitionVenuesJoin>(entity =>
            {
                entity.HasKey(e => new { e.CompetitionId, e.VenueId })
                    .HasName("PK_dbo.CompetitionVenuesJoin");

                entity.HasIndex(e => e.CompetitionId)
                    .HasName("IX_Competition_Id");

                entity.HasIndex(e => e.VenueId)
                    .HasName("IX_Venue_Id");

                entity.Property(e => e.CompetitionId).HasColumnName("Competition_Id");

                entity.Property(e => e.VenueId).HasColumnName("Venue_Id");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.CompetitionVenuesJoin)
                    .HasForeignKey(d => d.CompetitionId)
                    .HasConstraintName("FK_dbo.CompetitionVenuesJoin_dbo.Competitions_Competition_Id");

                entity.HasOne(d => d.Venue)
                    .WithMany(p => p.CompetitionVenuesJoin)
                    .HasForeignKey(d => d.VenueId)
                    .HasConstraintName("FK_dbo.CompetitionVenuesJoin_dbo.Venues_Venue_Id");
            });

            modelBuilder.Entity<Competition>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'2015-01-20T00:00:00.000'");

                entity.Property(e => e.DateModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'2015-01-20T00:00:00.000'");

                entity.Property(e => e.EventId).HasDefaultValueSql("1");

                entity.Property(e => e.FlowState).HasDefaultValueSql("1");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");

                entity.Property(e => e.JudgeRequirementPerVenue).HasDefaultValueSql("1");

                entity.Property(e => e.TeamEntryFee)
                    .HasColumnType("decimal")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Competitions)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_dbo.Competitions_dbo.Events_EventId");
            });

            modelBuilder.Entity<DanceScore>(entity =>
            {
                entity.HasIndex(e => e.DanceId)
                    .HasName("IX_DanceId");

                entity.HasIndex(e => e.ScoreCategoryId)
                    .HasName("IX_ScoreCategoryId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.MarkGiven).HasColumnType("decimal");

                entity.HasOne(d => d.Dance)
                    .WithMany(p => p.DanceScores)
                    .HasForeignKey(d => d.DanceId)
                    .HasConstraintName("FK_dbo.DanceScores_dbo.Dances_DanceId");

                entity.HasOne(d => d.ScoreCategory)
                    .WithMany(p => p.DanceScores)
                    .HasForeignKey(d => d.ScoreCategoryId)
                    .HasConstraintName("FK_dbo.DanceScores_dbo.ScoreCategories_ScoreCategoryId");
            });

            modelBuilder.Entity<Dance>(entity =>
            {
                entity.HasIndex(e => e.CompetitionId)
                    .HasName("IX_CompetitionId");

                entity.HasIndex(e => e.TeamAttendanceId)
                    .HasName("IX_TeamAttendanceId");

                entity.HasIndex(e => e.VenueId)
                    .HasName("IX_VenueId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.DateScoresEntered).HasColumnType("datetime");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.Dances)
                    .HasForeignKey(d => d.CompetitionId)
                    .HasConstraintName("FK_dbo.Dances_dbo.Competitions_CompetitionId");

                entity.HasOne(d => d.TeamAttendance)
                    .WithMany(p => p.Dances)
                    .HasForeignKey(d => d.TeamAttendanceId)
                    .HasConstraintName("FK_dbo.Dances_dbo.TeamAttendances_TeamAttendanceId");

                entity.HasOne(d => d.Venue)
                    .WithMany(p => p.Dances)
                    .HasForeignKey(d => d.VenueId)
                    .HasConstraintName("FK_dbo.Dances_dbo.Venues_VenueId");
            });

            modelBuilder.Entity<DertCompetitionEntryAttributeDertCompetitionEntry>(entity =>
            {
                entity.HasKey(e => new { e.DertCompetitionEntryId, e.DertCompetitionEntryAttributeId })
                    .HasName("PK_dbo.DertCompetitionEntryAttributeDertCompetitionEntries");

                entity.HasIndex(e => e.DertCompetitionEntryAttributeId)
                    .HasName("IX_DertCompetitionEntryAttribute_Id");

                entity.HasIndex(e => e.DertCompetitionEntryId)
                    .HasName("IX_DertCompetitionEntry_Id");

                entity.Property(e => e.DertCompetitionEntryId).HasColumnName("DertCompetitionEntry_Id");

                entity.Property(e => e.DertCompetitionEntryAttributeId).HasColumnName("DertCompetitionEntryAttribute_Id");

                entity.HasOne(d => d.DertCompetitionEntryAttribute)
                    .WithMany(p => p.DertCompetitionEntryAttributeDertCompetitionEntries)
                    .HasForeignKey(d => d.DertCompetitionEntryAttributeId)
                    .HasConstraintName("FK_dbo.DertCompetitionEntryAttributeDertCompetitionEntries_dbo.CompetitionEntryAttributes_DertCompetitionEntryAttribute_Id");

                entity.HasOne(d => d.DertCompetitionEntry)
                    .WithMany(p => p.DertCompetitionEntryAttributeDertCompetitionEntries)
                    .HasForeignKey(d => d.DertCompetitionEntryId)
                    .HasConstraintName("FK_dbo.DertCompetitionEntryAttributeDertCompetitionEntries_dbo.CompetitionEntries_DertCompetitionEntry_Id");
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EmailTemplates)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_dbo.EmailTemplates_dbo.Events_EventId");
            });

            modelBuilder.Entity<EventImage>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageId");

                entity.Property(e => e.IsPrimary).HasDefaultValueSql("0");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventImages)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_dbo.EventImages_dbo.Events_EventId");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.EventImages)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.EventImages_dbo.Images_ImageId");
            });

            modelBuilder.Entity<EventJudge>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.HasIndex(e => e.JudgeId)
                    .HasName("IX_JudgeId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventJudges)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dbo.EventJudges_dbo.Events_EventId");

                entity.HasOne(d => d.Judge)
                    .WithMany(p => p.EventJudges)
                    .HasForeignKey(d => d.JudgeId)
                    .HasConstraintName("FK_dbo.EventJudges_dbo.Judges_JudgeId");
            });

            modelBuilder.Entity<CompetitionJudge>(entity =>
            {
                entity.HasIndex(e => e.CompetitionId)
                    .HasName("IX_CompetitionId");

                entity.HasIndex(e => e.JudgeId)
                    .HasName("IX_JudgeId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.CompetitionJudges)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dbo.CompetitionJudges_dbo.Competitions_CompetitionId");

                entity.HasOne(d => d.Judge)
                    .WithMany(p => p.CompetitionJudges)
                    .HasForeignKey(d => d.JudgeId)
                    .HasConstraintName("FK_dbo.CompetitionJudges_dbo.Judges_JudgeId");
            });

            modelBuilder.Entity<EventSetting>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventSettings)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_dbo.EventSettings_dbo.Events_EventId");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.EventEndDate).HasColumnType("datetime");

                entity.Property(e => e.EventStartDate).HasColumnType("datetime");

                entity.Property(e => e.RegistrationCloseDate).HasColumnType("datetime");

                entity.Property(e => e.RegistrationOpenDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<GroupImage>(entity =>
            {
                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_GroupId");

                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageId");

                entity.Property(e => e.IsPrimary).HasDefaultValueSql("0");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupImages)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.GroupImages_dbo.Groups_GroupId");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.GroupImages)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.GroupImages_dbo.Images_ImageId");
            });

            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_GroupId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupMembers)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.GroupMembers_dbo.Groups_GroupId");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasIndex(e => e.RegistrationId)
                    .HasName("IX_RegistrationId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.InvoiceTotal).HasColumnType("decimal");

                entity.HasOne(d => d.Registration)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.RegistrationId)
                    .HasConstraintName("FK_dbo.Invoices_dbo.Registrations_GroupRegistrationId");
            });

            modelBuilder.Entity<JudgeSlot>(entity =>
            {
                entity.HasIndex(e => e.CompetitionId)
                    .HasName("IX_CompetitionId");

                entity.HasIndex(e => e.JudgeId)
                    .HasName("IX_JudgeId");

                entity.HasIndex(e => e.ScoreSetId)
                    .HasName("IX_ScoreSetId");

                entity.HasIndex(e => e.VenueId)
                    .HasName("IX_VenueId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.JudgeSlots)
                    .HasForeignKey(d => d.CompetitionId)
                    .HasConstraintName("FK_dbo.Judges_dbo.Competitions_CompetitionId");

                entity.HasOne(d => d.Judge)
                    .WithMany(p => p.JudgeSlots)
                    .HasForeignKey(d => d.JudgeId)
                    .HasConstraintName("FK_dbo.JudgeSlots_dbo.Judges_JudgeId");

                entity.HasOne(d => d.ScoreSet)
                    .WithMany(p => p.JudgeSlots)
                    .HasForeignKey(d => d.ScoreSetId)
                    .HasConstraintName("FK_dbo.JudgeSlots_dbo.ScoreSets_ScoreSetId");

                entity.HasOne(d => d.Venue)
                    .WithMany(p => p.JudgeSlots)
                    .HasForeignKey(d => d.VenueId)
                    .HasConstraintName("FK_dbo.Judges_dbo.Venues_VenueId");
            });

            modelBuilder.Entity<Judge>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarkingSheetImage>(entity =>
            {
                entity.HasIndex(e => e.DanceId)
                    .HasName("IX_DanceId");

                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageId");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1900-01-01T00:00:00.000'");

                entity.Property(e => e.DateModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'1900-01-01T00:00:00.000'");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");

                entity.HasOne(d => d.Dance)
                    .WithMany(p => p.MarkingSheetImages)
                    .HasForeignKey(d => d.DanceId)
                    .HasConstraintName("FK_dbo.MarkingSheetImages_dbo.Dances_DanceId");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.MarkingSheetImages)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.MarkingSheetImages_dbo.Images_ImageId");
            });

            modelBuilder.Entity<MarkingSheet>(entity =>
            {
                entity.HasIndex(e => e.DanceId)
                    .HasName("IX_DanceId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Dance)
                    .WithMany(p => p.MarkingSheets)
                    .HasForeignKey(d => d.DanceId)
                    .HasConstraintName("FK_dbo.MarkingSheets_dbo.Dances_DanceId");
            });

            modelBuilder.Entity<MemberAttendance>(entity =>
            {
                entity.HasIndex(e => e.AttendanceClassificationId)
                    .HasName("IX_AttendanceClassificationId");

                entity.HasIndex(e => e.GroupMemberId)
                    .HasName("IX_GroupMemberId");

                entity.HasIndex(e => e.RegistrationId)
                    .HasName("IX_RegistrationId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.AttendanceClassification)
                    .WithMany(p => p.MemberAttendances)
                    .HasForeignKey(d => d.AttendanceClassificationId)
                    .HasConstraintName("FK_dbo.MemberAttendances_dbo.AttendanceClassifications_AttendanceClassificationId");

                entity.HasOne(d => d.GroupMember)
                    .WithMany(p => p.MemberAttendances)
                    .HasForeignKey(d => d.GroupMemberId)
                    .HasConstraintName("FK_dbo.MemberAttendances_dbo.GroupMembers_GroupMemberId");

                entity.HasOne(d => d.Registration)
                    .WithMany(p => p.MemberAttendances)
                    .HasForeignKey(d => d.RegistrationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dbo.MemberAttendances_dbo.Registrations_RegistrationId");
            });

            modelBuilder.Entity<NavigationItem>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_GroupId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.FlowState).HasDefaultValueSql("0");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Registrations)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_dbo.Registrations_dbo.Events_EventId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Registrations)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.Registrations_dbo.Groups_GroupId");
            });

            modelBuilder.Entity<ScoreCategory>(entity =>
            {
                entity.HasIndex(e => e.CompetitionAppliesToId)
                    .HasName("IX_CompetitionAppliesToId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.CompetitionAppliesTo)
                    .WithMany(p => p.ScoreCategories)
                    .HasForeignKey(d => d.CompetitionAppliesToId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dbo.ScoreCategories_dbo.Competitions_CompetitionAppliesToId");
            });

            modelBuilder.Entity<ScoreSetScoreCategory>(entity =>
            {
                entity.HasKey(e => new { e.ScoreSetId, e.ScoreCategoryId })
                    .HasName("PK_dbo.ScoreSetScoreCategories");

                entity.HasIndex(e => e.ScoreCategoryId)
                    .HasName("IX_ScoreCategory_Id");

                entity.HasIndex(e => e.ScoreSetId)
                    .HasName("IX_ScoreSet_Id");

                entity.Property(e => e.ScoreSetId).HasColumnName("ScoreSet_Id");

                entity.Property(e => e.ScoreCategoryId).HasColumnName("ScoreCategory_Id");

                entity.HasOne(d => d.ScoreCategory)
                    .WithMany(p => p.ScoreSetScoreCategories)
                    .HasForeignKey(d => d.ScoreCategoryId)
                    .HasConstraintName("FK_dbo.ScoreSetScoreCategories_dbo.ScoreCategories_ScoreCategory_Id");

                entity.HasOne(d => d.ScoreSet)
                    .WithMany(p => p.ScoreSetScoreCategories)
                    .HasForeignKey(d => d.ScoreSetId)
                    .HasConstraintName("FK_dbo.ScoreSetScoreCategories_dbo.ScoreSets_ScoreSet_Id");
            });

            modelBuilder.Entity<ScoreSet>(entity =>
            {
                entity.HasIndex(e => e.CompetitionId)
                    .HasName("IX_CompetitionId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.ScoreSets)
                    .HasForeignKey(d => d.CompetitionId)
                    .HasConstraintName("FK_dbo.ScoreSets_dbo.Competitions_CompetitionId");
            });

            modelBuilder.Entity<Spectator>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<StaticResult>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<Steward>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");
            });

            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");
            });

            modelBuilder.Entity<TeamAggregateScore>(entity =>
            {
                entity.HasIndex(e => e.DertTeamId)
                    .HasName("IX_DertTeamId");

                entity.Property(e => e.AggregateScore).HasColumnType("decimal");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.DertTeam)
                    .WithMany(p => p.TeamAggregateScores)
                    .HasForeignKey(d => d.DertTeamId)
                    .HasConstraintName("FK_dbo.TeamAggregateScores_dbo.Teams_DertTeamId");
            });

            modelBuilder.Entity<TeamAttendance>(entity =>
            {
                entity.HasIndex(e => e.RegistrationId)
                    .HasName("IX_RegistrationId");

                entity.HasIndex(e => e.TeamId)
                    .HasName("IX_TeamId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.HasOne(d => d.Registration)
                    .WithMany(p => p.TeamAttendances)
                    .HasForeignKey(d => d.RegistrationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dbo.TeamAttendances_dbo.Registrations_RegistrationId");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamAttendances)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_dbo.TeamAttendances_dbo.Teams_TeamId");
            });

            modelBuilder.Entity<TeamImage>(entity =>
            {
                entity.HasIndex(e => e.ImageId)
                    .HasName("IX_ImageId");

                entity.HasIndex(e => e.TeamId)
                    .HasName("IX_TeamId");

                entity.Property(e => e.IsPrimary).HasDefaultValueSql("0");

                entity.HasOne(d => d.Image)
                    .WithMany(p => p.TeamImages)
                    .HasForeignKey(d => d.ImageId)
                    .HasConstraintName("FK_dbo.TeamImages_dbo.Images_ImageId");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamImages)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_dbo.TeamImages_dbo.Teams_TeamId");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasIndex(e => e.GroupId)
                    .HasName("IX_GroupId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("0");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_dbo.Teams_dbo.Groups_GroupId");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_dbo.UserProfile");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasDefaultValueSql("'Should be your username.'");

                entity.Property(e => e.YourName)
                    .IsRequired()
                    .HasDefaultValueSql("'Should be your name.'");
            });

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasIndex(e => e.EventId)
                    .HasName("IX_EventId");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.EventId).HasDefaultValueSql("1");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Venues)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dbo.Venues_dbo.Events_EventId");
            });

            modelBuilder.Entity<WebpagesMembership>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__webpages__1788CC4CE7FC2C39");

                entity.ToTable("webpages_Membership");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.ConfirmationToken).HasMaxLength(128);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.IsConfirmed).HasDefaultValueSql("0");

                entity.Property(e => e.LastPasswordFailureDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordChangedDate).HasColumnType("datetime");

                entity.Property(e => e.PasswordFailuresSinceLastSuccess).HasDefaultValueSql("0");

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.PasswordVerificationToken).HasMaxLength(128);

                entity.Property(e => e.PasswordVerificationTokenExpirationDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WebpagesOauthMembership>(entity =>
            {
                entity.HasKey(e => new { e.Provider, e.ProviderUserId })
                    .HasName("PK__webpages__F53FC0ED8537BC39");

                entity.ToTable("webpages_OAuthMembership");

                entity.Property(e => e.Provider).HasMaxLength(30);

                entity.Property(e => e.ProviderUserId).HasMaxLength(100);
            });

            modelBuilder.Entity<WebpagesRoles>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK__webpages__8AFACE1A5DFE3B86");

                entity.ToTable("webpages_Roles");

                entity.HasIndex(e => e.RoleName)
                    .HasName("UQ__webpages__8A2B6160B736E32A")
                    .IsUnique();

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<WebpagesUsersInRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK__webpages__AF2760ADE5219DA6");

                entity.ToTable("webpages_UsersInRoles");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.WebpagesUsersInRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WebpagesUsersInRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("fk_UserId");
            });

            /*
             * David Hall Notes
             * These model query filter definitions are applied to every instance of where a type is queried.
             * Ref: https://docs.microsoft.com/en-us/ef/core/what-is-new/#model-level-query-filters
             * This supports the soft delete of entities and allows omission of those entities when the application context queries
             *  those entities.
             *  In individual queries where these items are required we add .IgnoreQueryFilters()
             */

            modelBuilder.Entity<GroupMember>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Team>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Group>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Event>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Image>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Invoice>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Registration>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<TeamAttendance>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<MemberAttendance>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ActivityTeamAttendance>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ActivityMemberAttendance>().HasQueryFilter(x => !x.IsDeleted);
        }
    }
}