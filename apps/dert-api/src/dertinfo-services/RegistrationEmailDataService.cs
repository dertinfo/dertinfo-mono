using DertInfo.Models.Emails;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IRegistrationEmailDataService
    {
        Task<GroupRegistationConfirmationEmailData> BuildGroupRegistrationConfirmationData(int registrationId);

        Task<GroupRegistationSubmissionEmailData> BuildGroupRegistrationSubmissionData(int registrationId);

        Task<EventCancellationEmailData> BuildEventCancellationData(int registrationId);
    }

    public class RegistrationEmailDataService : IRegistrationEmailDataService
    {
        IEventSettingRepository _eventSettingsRepository;
        IEmailTemplateRepository _emailTemplateRepository;
        IRegistrationRepository _registrationRepository;

        public RegistrationEmailDataService(
            IEventSettingRepository eventSettingsRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IRegistrationRepository registrationRepository
            )
        {
            _eventSettingsRepository = eventSettingsRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _registrationRepository = registrationRepository;
        }

        public async Task<EventCancellationEmailData> BuildEventCancellationData(int registrationId)
        {
            // Get variables
            var registration = await this._registrationRepository.GetForEmail(registrationId);
            var eventId = registration.Event.Id;
            var groupEmailAddress = registration.Group.PrimaryContactEmail;
            var eventEmailAddress = registration.Event.ContactEmail;
            var eventEmailName = registration.Event.ContactName;
            var eventName = registration.Event.Name;
            List<string> bcc = new List<string>();

            // Identify the template
            var emailTemplates = await this._emailTemplateRepository.Find(et => et.EventId == eventId && et.TemplateRef == EmailTemplateType.EVENT_CANCELLATION.ToString());
            var emailTemplateId = emailTemplates.First().Id;

            // Populate Bcc
            if (registration.Event.SentEmailsBcc != null)
            {
                var strEmailBccs = registration.Event.SentEmailsBcc.Split(',');
                foreach (var emailBcc in strEmailBccs)
                {
                    if (emailBcc.Trim() != string.Empty)
                    {
                        bcc.Add(emailBcc);
                    }
                }
            }

            // Build Data Object
            var eventCancellation = new EventCancellationEmailData();
            eventCancellation.ToAddresses = new string[] { groupEmailAddress };
            eventCancellation.FromAddress = eventEmailAddress;
            eventCancellation.FromName = eventEmailName;
            eventCancellation.Subject = "@:" + eventName; //@: indicates to use after the template subject
            eventCancellation.EmailTemplateId = emailTemplateId;
            eventCancellation.BccAddresses = bcc.ToArray();

            eventCancellation.GroupName = registration.Group.GroupName;
            eventCancellation.ContactName = registration.Group.PrimaryContactName;
            eventCancellation.EventName = registration.Event.Name;

            return eventCancellation;
        }

        public async Task<GroupRegistationConfirmationEmailData> BuildGroupRegistrationConfirmationData(int registrationId)
        {
            // Get variables
            var registration = await this._registrationRepository.GetForEmail(registrationId);
            var eventId = registration.Event.Id;
            var groupEmailAddress = registration.Group.PrimaryContactEmail;
            var eventEmailAddress = registration.Event.ContactEmail;
            var eventEmailName = registration.Event.ContactName;
            var eventName = registration.Event.Name;
            List<string> bcc = new List<string>();

            //Identify the template
            var emailTemplates = await this._emailTemplateRepository.Find(et => et.EventId == eventId && et.TemplateRef == EmailTemplateType.REGISTRATION_CONFIRM.ToString());
            var emailTemplateId = emailTemplates.First().Id;

            // Populate Bcc
            if (registration.Event.SentEmailsBcc != null)
            {
                var strEmailBccs = registration.Event.SentEmailsBcc.Split(',');
                foreach (var emailBcc in strEmailBccs)
                {
                    if (emailBcc.Trim() != string.Empty)
                    {
                        bcc.Add(emailBcc);
                    }
                }
            }

            // Build Data Object
            var registrationSubmission = new GroupRegistationConfirmationEmailData();
            registrationSubmission.ToAddresses = new string[] { groupEmailAddress };
            registrationSubmission.FromAddress = eventEmailAddress;
            registrationSubmission.FromName = eventEmailName;
            registrationSubmission.Subject = "@:" + eventName; //@: indicates to use after the template subject
            registrationSubmission.EmailTemplateId = emailTemplateId;
            registrationSubmission.BccAddresses = bcc.ToArray();

            registrationSubmission.GroupName = registration.Group.GroupName;
            registrationSubmission.ContactName = registration.Group.PrimaryContactName;
            registrationSubmission.ContactNumber = registration.Group.PrimaryContactNumber;

            registrationSubmission.EventName = registration.Event.Name;
            registrationSubmission.EventRegistrationCloseDate = registration.Event.RegistrationCloseDate ?? (DateTime)registration.Event.EventStartDate;

            foreach (var attendingMember in registration.MemberAttendances.Where(am => !am.IsDeleted))
            {
                IndividualAttendanceLineItem individualAttendanceLineItem = new IndividualAttendanceLineItem();
                individualAttendanceLineItem.FullName = attendingMember.GroupMember.Name;

                foreach (var attendanceActivity in attendingMember.MemberActivities)
                {
                    ActivityLineItem activityLineItem = new ActivityLineItem();

                    activityLineItem.ActivityName = attendanceActivity.Activity.Title;
                    activityLineItem.ActivityPrice = attendanceActivity.Activity.Price;

                    individualAttendanceLineItem.Activities.Add(activityLineItem);
                }

                registrationSubmission.IndividualAttendanceLineItems.Add(individualAttendanceLineItem);
            }

            foreach (var attendingTeam in registration.TeamAttendances.Where(at => !at.IsDeleted))
            {
                TeamAttendanceLineItem teamAttendanceLineItem = new TeamAttendanceLineItem();
                teamAttendanceLineItem.TeamName = attendingTeam.Team.TeamName;

                foreach (var attendanceActivity in attendingTeam.TeamActivities)
                {
                    if (!attendanceActivity.IsDeleted)
                    {
                        ActivityLineItem activityLineItem = new ActivityLineItem();

                        activityLineItem.ActivityName = attendanceActivity.Activity.Title;
                        activityLineItem.ActivityPrice = attendanceActivity.Activity.Price;

                        teamAttendanceLineItem.Activities.Add(activityLineItem);
                    }
                }

                registrationSubmission.TeamAttendanceLineItems.Add(teamAttendanceLineItem);
            }

            return registrationSubmission;
        }

        /// <summary>
        /// note - At this point this is the same as the confirmation. It just won't show the price breakdown in the email.
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        public async Task<GroupRegistationSubmissionEmailData> BuildGroupRegistrationSubmissionData(int registrationId)
        {
            // Get variables
            var registration = await this._registrationRepository.GetForEmail(registrationId);
            var eventId = registration.Event.Id;
            var groupEmailAddress = registration.Group.PrimaryContactEmail;
            var eventEmailAddress = registration.Event.ContactEmail;
            var eventEmailName = registration.Event.ContactName;
            var eventName = registration.Event.Name;
            List<string> bcc = new List<string>();

            //Identify the template
            var emailTemplates = await this._emailTemplateRepository.Find(et => et.EventId == eventId && et.TemplateRef == EmailTemplateType.REGISTRATION_SUBMIT.ToString());
            var emailTemplateId = emailTemplates.First().Id;

            // Populate Bcc
            if (registration.Event.SentEmailsBcc != null)
            {
                var strEmailBccs = registration.Event.SentEmailsBcc.Split(',');
                foreach (var emailBcc in strEmailBccs)
                {
                    if (emailBcc.Trim() != string.Empty)
                    {
                        bcc.Add(emailBcc);
                    }
                }
            }

            // Build Data Object
            var registrationSubmission = new GroupRegistationSubmissionEmailData();
            registrationSubmission.ToAddresses = new string[] { groupEmailAddress };
            registrationSubmission.FromAddress = eventEmailAddress;
            registrationSubmission.FromName = eventEmailName;
            registrationSubmission.Subject = "@:" + eventName; //@: indicates to use after the template subject
            registrationSubmission.EmailTemplateId = emailTemplateId;
            registrationSubmission.BccAddresses = bcc.ToArray();

            registrationSubmission.GroupName = registration.Group.GroupName;
            registrationSubmission.ContactName = registration.Group.PrimaryContactName;
            registrationSubmission.ContactNumber = registration.Group.PrimaryContactNumber;

            registrationSubmission.EventName = registration.Event.Name;
            registrationSubmission.EventRegistrationCloseDate = registration.Event.RegistrationCloseDate ?? (DateTime)registration.Event.EventStartDate;

            foreach (var attendingMember in registration.MemberAttendances.Where(am => !am.IsDeleted))
            {
                IndividualAttendanceLineItem individualAttendanceLineItem = new IndividualAttendanceLineItem();
                individualAttendanceLineItem.FullName = attendingMember.GroupMember.Name;

                foreach (var attendanceActivity in attendingMember.MemberActivities)
                {
                    ActivityLineItem activityLineItem = new ActivityLineItem();

                    activityLineItem.ActivityName = attendanceActivity.Activity.Title;
                    activityLineItem.ActivityPrice = attendanceActivity.Activity.Price;

                    individualAttendanceLineItem.Activities.Add(activityLineItem);
                }

                registrationSubmission.IndividualAttendanceLineItems.Add(individualAttendanceLineItem);
            }

            foreach (var attendingTeam in registration.TeamAttendances.Where(at => !at.IsDeleted))
            {
                TeamAttendanceLineItem teamAttendanceLineItem = new TeamAttendanceLineItem();
                teamAttendanceLineItem.TeamName = attendingTeam.Team.TeamName;

                foreach (var attendanceActivity in attendingTeam.TeamActivities)
                {
                    if (!attendanceActivity.IsDeleted)
                    {
                        ActivityLineItem activityLineItem = new ActivityLineItem();

                        activityLineItem.ActivityName = attendanceActivity.Activity.Title;
                        activityLineItem.ActivityPrice = attendanceActivity.Activity.Price;

                        teamAttendanceLineItem.Activities.Add(activityLineItem);
                    }
                }

                registrationSubmission.TeamAttendanceLineItems.Add(teamAttendanceLineItem);
            }

            return registrationSubmission;
        }
    }
}
