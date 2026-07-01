using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DertInfo.Repository.Migrations
{
    public partial class InitialAfterReverseEngineer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessKey",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessKeyRef = table.Column<Guid>(nullable: false, defaultValueSql: "'00000000-0000-0000-0000-000000000000'"),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BreadcrumbItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Action = table.Column<string>(nullable: true),
                    Controller = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsTypeRoot = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    label = table.Column<string>(nullable: true),
                    lineageIndex = table.Column<int>(nullable: false),
                    ObjectId = table.Column<int>(nullable: false),
                    PageUri = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreadcrumbItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventEndDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'2015-04-12T00:00:00.000'"),
                    EventStartDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'2015-04-10T00:00:00.000'"),
                    EventSynopsis = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    RegistrationCloseDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RegistrationOpenDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    GroupBio = table.Column<string>(nullable: true),
                    GroupImageUrl = table.Column<string>(nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    ModifiedBy = table.Column<string>(nullable: true),
                    PrimaryContactEmail = table.Column<string>(nullable: true),
                    PrimaryContactName = table.Column<string>(nullable: true),
                    PrimaryContactNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    ImageAlt = table.Column<string>(nullable: true),
                    ImagePath = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Judges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    Email = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Telephone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "__MigrationHistory",
                columns: table => new
                {
                    MigrationId = table.Column<string>(maxLength: 150, nullable: false),
                    ContextKey = table.Column<string>(maxLength: 300, nullable: false),
                    Model = table.Column<byte[]>(nullable: false),
                    ProductVersion = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.__MigrationHistory", x => new { x.MigrationId, x.ContextKey });
                });

            migrationBuilder.CreateTable(
                name: "NavigationItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    MinimumRequiredRole = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NavigationItemSpecialRef = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UserAppliesTo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Spectators",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DertYear = table.Column<int>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    NumberOfAdultConcessionTickets = table.Column<int>(nullable: false),
                    NumberOfAdultTickets = table.Column<int>(nullable: false),
                    NumberOfCampingTickets = table.Column<int>(nullable: false),
                    NumberOfJuniorTickets = table.Column<int>(nullable: false),
                    NumberOfYouthTickets = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spectators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaticResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    HtmlContent = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ResultType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stewards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContactNumber = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DertYear = table.Column<int>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stewards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Ref = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: false, defaultValueSql: "'Should be your username.'"),
                    YourName = table.Column<string>(nullable: false, defaultValueSql: "'Should be your name.'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.UserProfile", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "webpages_Membership",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    ConfirmationToken = table.Column<string>(maxLength: 128, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: true, defaultValueSql: "0"),
                    LastPasswordFailureDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Password = table.Column<string>(maxLength: 128, nullable: false),
                    PasswordChangedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PasswordFailuresSinceLastSuccess = table.Column<int>(nullable: false, defaultValueSql: "0"),
                    PasswordSalt = table.Column<string>(maxLength: 128, nullable: false),
                    PasswordVerificationToken = table.Column<string>(maxLength: 128, nullable: true),
                    PasswordVerificationTokenExpirationDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__webpages__1788CC4CE7FC2C39", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "webpages_OAuthMembership",
                columns: table => new
                {
                    Provider = table.Column<string>(maxLength: 30, nullable: false),
                    ProviderUserId = table.Column<string>(maxLength: 100, nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__webpages__F53FC0ED8537BC39", x => new { x.Provider, x.ProviderUserId });
                });

            migrationBuilder.CreateTable(
                name: "webpages_Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__webpages__8AFACE1A5DFE3B86", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "AccessKeyUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessKeyId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DeletePermitted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    EditPermitted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    ViewPermitted = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessKeyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.AccessKeyUsers_dbo.AccessKeys_AccessKeyId",
                        column: x => x.AccessKeyId,
                        principalTable: "AccessKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceClassifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    ClassificationName = table.Column<string>(nullable: true),
                    ClassificationPrice = table.Column<decimal>(type: "decimal", nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceClassifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.AttendanceClassifications_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CompetitionDescription = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'2015-01-20T00:00:00.000'"),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'2015-01-20T00:00:00.000'"),
                    EventId = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    FlowState = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    JudgeRequirementPerVenue = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    TeamEntryFee = table.Column<decimal>(type: "decimal", nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Competitions_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    TemplateName = table.Column<string>(nullable: true),
                    TemplateRef = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.EmailTemplates_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Ref = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.EventSettings_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventId = table.Column<int>(nullable: false, defaultValueSql: "1"),
                    IsDeleted = table.Column<bool>(nullable: false),
                    JudgeMinderUsername = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Venues_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime", nullable: true),
                    GroupId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.GroupMembers_dbo.Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DertYear = table.Column<int>(nullable: false),
                    EstimateAccomodation = table.Column<int>(nullable: false),
                    EstimateAttending = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    FlowState = table.Column<int>(nullable: false, defaultValueSql: "0"),
                    GroupId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    ModifiedBy = table.Column<string>(nullable: true),
                    SpecialRequirements = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Registrations_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Registrations_dbo.Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TeamBio = table.Column<string>(nullable: true),
                    TeamName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Teams_dbo.Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(nullable: false),
                    ImageId = table.Column<int>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.EventImages_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.EventImages_dbo.Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<int>(nullable: false),
                    ImageId = table.Column<int>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.GroupImages_dbo.Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.GroupImages_dbo.Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventJudges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    JudgeId = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventJudges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.EventJudges_dbo.Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.EventJudges_dbo.Judges_JudgeId",
                        column: x => x.JudgeId,
                        principalTable: "Judges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "webpages_UsersInRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__webpages__AF2760ADE5219DA6", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "fk_RoleId",
                        column: x => x.RoleId,
                        principalTable: "webpages_Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfile",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionEntryAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CompetitionAppliesToId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionEntryAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.CompetitionEntryAttributes_dbo.Competitions_CompetitionAppliesToId",
                        column: x => x.CompetitionAppliesToId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScoreCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CompetitionAppliesToId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    InScoreSet1 = table.Column<bool>(nullable: false),
                    InScoreSet2 = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MaxMarks = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SortOrder = table.Column<int>(nullable: false),
                    Tag = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ScoreCategories_dbo.Competitions_CompetitionAppliesToId",
                        column: x => x.CompetitionAppliesToId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScoreSets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CompetitionId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScoreSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.ScoreSets_dbo.Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionVenuesJoin",
                columns: table => new
                {
                    Competition_Id = table.Column<int>(nullable: false),
                    Venue_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.CompetitionVenuesJoin", x => new { x.Competition_Id, x.Venue_Id });
                    table.ForeignKey(
                        name: "FK_dbo.CompetitionVenuesJoin_dbo.Competitions_Competition_Id",
                        column: x => x.Competition_Id,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.CompetitionVenuesJoin_dbo.Venues_Venue_Id",
                        column: x => x.Venue_Id,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    HasPaid = table.Column<bool>(nullable: false),
                    InvoiceCode = table.Column<string>(nullable: true),
                    InvoiceEntryNotes = table.Column<string>(nullable: true),
                    InvoiceLineItemNotes = table.Column<string>(nullable: true),
                    InvoiceToEmail = table.Column<string>(nullable: true),
                    InvoiceToName = table.Column<string>(nullable: true),
                    InvoiceToTeamName = table.Column<string>(nullable: true),
                    InvoiceTotal = table.Column<decimal>(type: "decimal", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    RegistrationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Invoices_dbo.Registrations_GroupRegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    AttendanceClassificationId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    GroupMemberId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    RegistrationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MemberAttendances_dbo.AttendanceClassifications_AttendanceClassificationId",
                        column: x => x.AttendanceClassificationId,
                        principalTable: "AttendanceClassifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.MemberAttendances_dbo.GroupMembers_GroupMemberId",
                        column: x => x.GroupMemberId,
                        principalTable: "GroupMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.MemberAttendances_dbo.Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamAggregateScores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AggregateScore = table.Column<decimal>(type: "decimal", nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DertTeamId = table.Column<int>(nullable: false),
                    DertYear = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Organiser = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAggregateScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.TeamAggregateScores_dbo.Teams_DertTeamId",
                        column: x => x.DertTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamAttendances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    AttendanceConfirmed = table.Column<bool>(nullable: false),
                    AttendanceNotes = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    RegistrationId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.TeamAttendances_dbo.Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.TeamAttendances_dbo.Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImageId = table.Column<int>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.TeamImages_dbo.Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.TeamImages_dbo.Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JudgeSlots",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CompetitionId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    JudgeId = table.Column<int>(nullable: true),
                    MarkingSet = table.Column<int>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ScoreSetId = table.Column<int>(nullable: true),
                    VenueId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JudgeSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Judges_dbo.Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.JudgeSlots_dbo.Judges_JudgeId",
                        column: x => x.JudgeId,
                        principalTable: "Judges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.JudgeSlots_dbo.ScoreSets_ScoreSetId",
                        column: x => x.ScoreSetId,
                        principalTable: "ScoreSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_dbo.Judges_dbo.Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScoreSetScoreCategories",
                columns: table => new
                {
                    ScoreSet_Id = table.Column<int>(nullable: false),
                    ScoreCategory_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.ScoreSetScoreCategories", x => new { x.ScoreSet_Id, x.ScoreCategory_Id });
                    table.ForeignKey(
                        name: "FK_dbo.ScoreSetScoreCategories_dbo.ScoreCategories_ScoreCategory_Id",
                        column: x => x.ScoreCategory_Id,
                        principalTable: "ScoreCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.ScoreSetScoreCategories_dbo.ScoreSets_ScoreSet_Id",
                        column: x => x.ScoreSet_Id,
                        principalTable: "ScoreSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CompetitionId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'2015-02-18T00:00:00.000'"),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'2015-02-18T00:00:00.000'"),
                    DertYear = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    IsDisabled = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    TeamAttendanceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.CompetitionEntries_dbo.Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.CompetitionEntries_dbo.TeamAttendances_TeamAttendanceId",
                        column: x => x.TeamAttendanceId,
                        principalTable: "TeamAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CompetitionId = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateScoresEntered = table.Column<DateTime>(type: "datetime", nullable: true),
                    DertYear = table.Column<int>(nullable: false),
                    HasScoresChecked = table.Column<bool>(nullable: false),
                    HasScoresEntered = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    RandomTest = table.Column<bool>(nullable: false),
                    ScoresEnteredBy = table.Column<string>(nullable: true),
                    TeamAttendanceId = table.Column<int>(nullable: false),
                    VenueId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.Dances_dbo.Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Dances_dbo.TeamAttendances_TeamAttendanceId",
                        column: x => x.TeamAttendanceId,
                        principalTable: "TeamAttendances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.Dances_dbo.Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DertCompetitionEntryAttributeDertCompetitionEntries",
                columns: table => new
                {
                    DertCompetitionEntry_Id = table.Column<int>(nullable: false),
                    DertCompetitionEntryAttribute_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dbo.DertCompetitionEntryAttributeDertCompetitionEntries", x => new { x.DertCompetitionEntry_Id, x.DertCompetitionEntryAttribute_Id });
                    table.ForeignKey(
                        name: "FK_dbo.DertCompetitionEntryAttributeDertCompetitionEntries_dbo.CompetitionEntryAttributes_DertCompetitionEntryAttribute_Id",
                        column: x => x.DertCompetitionEntryAttribute_Id,
                        principalTable: "CompetitionEntryAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DertCompetitionEntryAttributeDertCompetitionEntries_dbo.CompetitionEntries_DertCompetitionEntry_Id",
                        column: x => x.DertCompetitionEntry_Id,
                        principalTable: "CompetitionEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanceScores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentGiven = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DanceId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    MarkGiven = table.Column<decimal>(type: "decimal", nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ScoreCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanceScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.DanceScores_dbo.Dances_DanceId",
                        column: x => x.DanceId,
                        principalTable: "Dances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.DanceScores_dbo.ScoreCategories_ScoreCategoryId",
                        column: x => x.ScoreCategoryId,
                        principalTable: "ScoreCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarkingSheetImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DanceId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'1900-01-01T00:00:00.000'"),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "'1900-01-01T00:00:00.000'"),
                    ImageId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValueSql: "0"),
                    ModifiedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkingSheetImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MarkingSheetImages_dbo.Dances_DanceId",
                        column: x => x.DanceId,
                        principalTable: "Dances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dbo.MarkingSheetImages_dbo.Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarkingSheets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessToken = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DanceId = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModifiedBy = table.Column<string>(nullable: true),
                    ScoreSheetImageUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarkingSheets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dbo.MarkingSheets_dbo.Dances_DanceId",
                        column: x => x.DanceId,
                        principalTable: "Dances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessKeyId",
                table: "AccessKeyUsers",
                column: "AccessKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "AttendanceClassifications",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionId",
                table: "CompetitionEntries",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAttendanceId",
                table: "CompetitionEntries",
                column: "TeamAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionAppliesToId",
                table: "CompetitionEntryAttributes",
                column: "CompetitionAppliesToId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "Competitions",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Competition_Id",
                table: "CompetitionVenuesJoin",
                column: "Competition_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Venue_Id",
                table: "CompetitionVenuesJoin",
                column: "Venue_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionId",
                table: "Dances",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamAttendanceId",
                table: "Dances",
                column: "TeamAttendanceId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueId",
                table: "Dances",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_DanceId",
                table: "DanceScores",
                column: "DanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCategoryId",
                table: "DanceScores",
                column: "ScoreCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DertCompetitionEntryAttribute_Id",
                table: "DertCompetitionEntryAttributeDertCompetitionEntries",
                column: "DertCompetitionEntryAttribute_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DertCompetitionEntry_Id",
                table: "DertCompetitionEntryAttributeDertCompetitionEntries",
                column: "DertCompetitionEntry_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "EmailTemplates",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "EventImages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageId",
                table: "EventImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "EventJudges",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_JudgeId",
                table: "EventJudges",
                column: "JudgeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "EventSettings",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupId",
                table: "GroupImages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageId",
                table: "GroupImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupId",
                table: "GroupMembers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationId",
                table: "Invoices",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionId",
                table: "JudgeSlots",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_JudgeId",
                table: "JudgeSlots",
                column: "JudgeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreSetId",
                table: "JudgeSlots",
                column: "ScoreSetId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueId",
                table: "JudgeSlots",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_DanceId",
                table: "MarkingSheetImages",
                column: "DanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageId",
                table: "MarkingSheetImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DanceId",
                table: "MarkingSheets",
                column: "DanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceClassificationId",
                table: "MemberAttendances",
                column: "AttendanceClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberId",
                table: "MemberAttendances",
                column: "GroupMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationId",
                table: "MemberAttendances",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "Registrations",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupId",
                table: "Registrations",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionAppliesToId",
                table: "ScoreCategories",
                column: "CompetitionAppliesToId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionId",
                table: "ScoreSets",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreCategory_Id",
                table: "ScoreSetScoreCategories",
                column: "ScoreCategory_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ScoreSet_Id",
                table: "ScoreSetScoreCategories",
                column: "ScoreSet_Id");

            migrationBuilder.CreateIndex(
                name: "IX_DertTeamId",
                table: "TeamAggregateScores",
                column: "DertTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrationId",
                table: "TeamAttendances",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamId",
                table: "TeamAttendances",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageId",
                table: "TeamImages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamId",
                table: "TeamImages",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupId",
                table: "Teams",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EventId",
                table: "Venues",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "UQ__webpages__8A2B6160B736E32A",
                table: "webpages_Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_webpages_UsersInRoles_RoleId",
                table: "webpages_UsersInRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_webpages_UsersInRoles_UserId",
                table: "webpages_UsersInRoles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessKeyUsers");

            migrationBuilder.DropTable(
                name: "BreadcrumbItems");

            migrationBuilder.DropTable(
                name: "CompetitionVenuesJoin");

            migrationBuilder.DropTable(
                name: "DanceScores");

            migrationBuilder.DropTable(
                name: "DertCompetitionEntryAttributeDertCompetitionEntries");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "EventImages");

            migrationBuilder.DropTable(
                name: "EventJudges");

            migrationBuilder.DropTable(
                name: "EventSettings");

            migrationBuilder.DropTable(
                name: "GroupImages");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "JudgeSlots");

            migrationBuilder.DropTable(
                name: "MarkingSheetImages");

            migrationBuilder.DropTable(
                name: "MarkingSheets");

            migrationBuilder.DropTable(
                name: "MemberAttendances");

            migrationBuilder.DropTable(
                name: "__MigrationHistory");

            migrationBuilder.DropTable(
                name: "NavigationItems");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ScoreSetScoreCategories");

            migrationBuilder.DropTable(
                name: "Spectators");

            migrationBuilder.DropTable(
                name: "StaticResults");

            migrationBuilder.DropTable(
                name: "Stewards");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TeamAggregateScores");

            migrationBuilder.DropTable(
                name: "TeamImages");

            migrationBuilder.DropTable(
                name: "webpages_Membership");

            migrationBuilder.DropTable(
                name: "webpages_OAuthMembership");

            migrationBuilder.DropTable(
                name: "webpages_UsersInRoles");

            migrationBuilder.DropTable(
                name: "AccessKey");

            migrationBuilder.DropTable(
                name: "CompetitionEntryAttributes");

            migrationBuilder.DropTable(
                name: "CompetitionEntries");

            migrationBuilder.DropTable(
                name: "Judges");

            migrationBuilder.DropTable(
                name: "Dances");

            migrationBuilder.DropTable(
                name: "AttendanceClassifications");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "ScoreCategories");

            migrationBuilder.DropTable(
                name: "ScoreSets");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "webpages_Roles");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "TeamAttendances");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
