using System.Linq;
using DertInfo.Models.Database;
using System;
using Microsoft.EntityFrameworkCore;
using DertInfo.Models.System.Enumerations;

namespace DertInfo.Repository
{
    public static class DertInfoContextExtensions
    {
        public static void EnsureSeedData(this DertInfoContext context)
        {
            if (context.AllMigrationsApplied())
            {
                if (!context.SystemSettings.Any())
                {

                    /* Deprecated
                     * This setting is used to lookup in the file structure the correct templates and locations for files. 
                     * This mechanism is no longer used however some code in the web system still relys on this value.
                     */
                    var dertYear = new SystemSetting
                    {
                        Ref = "SYSTEM-DERTYEAR",
                        Name = "System - DertYear",
                        Value = DateTime.Now.Year.ToString(),
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(dertYear);

                    /* Deprecated
                     * I believe that this setting  is now in most of the configuration transforms
                     */
                    var cloudStorageContainer = new SystemSetting
                    {
                        Ref = "CloudStorageContainer",
                        Name = "System - CloudStorageContainer",
                        Value = "liveuserfiles",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(cloudStorageContainer);

                    /* 
                     * Used for sending emails. However each event now has it's own smtp server. 
                     */
                    var smtpServer = new SystemSetting
                    {
                        Ref = "SMTPServer",
                        Name = "SMTPServer",
                        Value = "smtp.gmail.com",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(smtpServer);

                    var smtpUsername = new SystemSetting
                    {
                        Ref = "SMTPUsername",
                        Name = "SMTPUsername",
                        Value = "[GMAIL ACCOUNT USER NAME]",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(smtpUsername);

                    var smtpPassword = new SystemSetting
                    {
                        Ref = "SMTPPassword",
                        Name = "SMTPPassword",
                        Value = "[GMAIL ACCOUNT PASSWORD]",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(smtpPassword);

                    var forgottenPasswordEmailTemplateId = new SystemSetting
                    {
                        Ref = "SYSTEM-FORGOTTEN-PASSWORD-EMAIL-TEMPLATE-ID",
                        Name = "Forgotten Password Email Id",
                        Value = "1",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(forgottenPasswordEmailTemplateId);

                    // todo - I'm not sure that this is required any longer. Validate that this system settings can be removed.
                    var judgeAccessToken = new SystemSetting
                    {
                        Ref = "SYSTEM-JUDGEACCESSTOKEN",
                        Name = "JudgeAccessToken",
                        Value = "2ee873af-5227-4a50-801f-74e027417474",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(judgeAccessToken);

                    var dertOfDertsResultsPublished = new SystemSetting
                    {
                        Ref = "SYSTEM-DOD-RESULTSPUBLISHED",
                        Name = "DERTofDERTs Results Published",
                        Value = "false",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(dertOfDertsResultsPublished);

                    var dertOfDertsOpenToPublic = new SystemSetting
                    {
                        Ref = "SYSTEM-DOD-OPENFORPUBLIC",
                        Name = "DERTofDERTs Open for public",
                        Value = "False",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(dertOfDertsOpenToPublic);

                    var dertOfDertsPublicResultsForwarded = new SystemSetting
                    {
                        Ref = "SYSTEM-DOD-PUBLICRESULTSFORWARDED",
                        Name = "DERTofDERTs Public Results Forwarded",
                        Value = "False",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(dertOfDertsPublicResultsForwarded);

                    var dertOfDertsOfficialResultsForwarded = new SystemSetting
                    {
                        Ref = "SYSTEM-DOD-OFFICIALRESULTSFORWARDED",
                        Name = "DERTofDERTs Official Results Forwarded",
                        Value = "False",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(dertOfDertsOfficialResultsForwarded);

                    var dertOfDertsJudgePasswords = new SystemSetting
                    {
                        Ref = "SYSTEM-DOD-JUDGEPASSWORDS",
                        Name = "Valid passwords to identify judges",
                        Value = "bluebell",
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        CreatedBy = "DataSeed",
                        ModifiedBy = "DataSeed",
                        IsDeleted = false
                    };
                    context.SystemSettings.Add(dertOfDertsJudgePasswords);

                    // Save the seed changes to the database.
                    context.SaveChanges();
                }

                if (!context.Activities.Any())
                {
                    // Migrate Competitions To Activities & ActivityTeamAttendances
                    var allCompetitions = context.Competitions
                        .Include(c => c.CompetitionEntries)
                        .ToList();

                    foreach (var comp in allCompetitions)
                    {

                        var activity = new Activity
                        {
                            CompetitionId = comp.Id,
                            Description = comp.CompetitionDescription,
                            Title = comp.Name,
                            Price = comp.TeamEntryFee,
                            EventId = comp.EventId,
                            DateCreated = comp.DateCreated,
                            DateModified = comp.DateModified,
                            CreatedBy = comp.CreatedBy,
                            ModifiedBy = comp.ModifiedBy,
                            IsDeleted = comp.IsDeleted,
                            AudienceTypeId = (int)ActivityAudienceType.TEAM
                        };

                        var activityTeamAttendances = comp.CompetitionEntries.Select(ce => new ActivityTeamAttendance
                        {
                            TeamAttendanceId = ce.TeamAttendanceId,
                            DateCreated = ce.DateCreated,
                            DateModified = ce.DateModified,
                            CreatedBy = ce.CreatedBy,
                            ModifiedBy = ce.ModifiedBy,
                            IsDeleted = ce.IsDisabled
                        });

                        activity.ParticipatingTeams = activityTeamAttendances.ToList();

                        context.Activities.Add(activity);
                    }

                    // Migrate Member Attendance Classifications To Activities & ActivityMemberAttendances
                    var allAttendanceClassfications = context.AttendanceClassifications
                        .Include(ac => ac.MemberAttendances)
                        .ToList();

                    foreach (var attCls in allAttendanceClassfications)
                    {
                        var activity = new Activity
                        {
                            CompetitionId = null,
                            Description = attCls.ClassificationName,
                            Title = attCls.ClassificationName,
                            Price = attCls.ClassificationPrice,
                            EventId = attCls.EventId,
                            DateCreated = attCls.DateCreated,
                            DateModified = attCls.DateModified,
                            CreatedBy = attCls.CreatedBy,
                            ModifiedBy = attCls.ModifiedBy,
                            IsDeleted = attCls.IsDeleted,
                            AudienceTypeId = (int)ActivityAudienceType.INDIVIDUAL
                        };

                        var activityMemberAttendances = attCls.MemberAttendances.Select(ma => new ActivityMemberAttendance
                        {
                            MemberAttendanceId = ma.Id,
                            DateCreated = ma.DateCreated,
                            DateModified = ma.DateModified,
                            CreatedBy = ma.CreatedBy,
                            ModifiedBy = ma.ModifiedBy,
                            IsDeleted = ma.IsDeleted
                        });

                        activity.ParticipatingIndividuals = activityMemberAttendances.ToList();

                        context.Activities.Add(activity);
                    }

                    // Save the seed changes to the database.
                    context.SaveChanges();
                }

                if (!context.CompetitionJudges.Any())
                {
                    // Migrate From Current Slot Allocations Into This Table So That we can get judges for the individual competitions

                    var existingJudgeSlotAllocations = context.JudgeSlots.ToList();

                    foreach (var judgeSlotAllocation in existingJudgeSlotAllocations)
                    {
                        if (judgeSlotAllocation.JudgeId != null)
                        {
                            var competitionJudge = new CompetitionJudge
                            {
                                CompetitionId = judgeSlotAllocation.CompetitionId,
                                JudgeId = (int)judgeSlotAllocation.JudgeId, // will not be null
                                DateCreated = judgeSlotAllocation.DateCreated,
                                DateModified = judgeSlotAllocation.DateModified,
                                CreatedBy = judgeSlotAllocation.CreatedBy,
                                ModifiedBy = judgeSlotAllocation.ModifiedBy,
                                IsDeleted = judgeSlotAllocation.IsDeleted
                            };

                            context.CompetitionJudges.Add(competitionJudge);
                        }
                    }

                    // Save the seed changes to the database.
                    context.SaveChanges();
                }
            }
        }
    }
}
