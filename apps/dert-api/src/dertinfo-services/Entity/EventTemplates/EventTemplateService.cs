using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using DertInfo.Services.Entity.CompetitionTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.EmailTemplates
{
    public interface IEventTemplateService
    {
        Task ApplyTemplate(Event myEvent, EventTemplateType templateType);
    }

    public class EventTemplateService : IEventTemplateService
    {
        // private IAttendanceClassificationRepository _attendanceClassificationRepository;
        private IActivityRepository _activityRepository;
        private ICompetitionTemplateService _competitionTemplateService;
        private IEmailTemplateService _emailTemplateService;
        private IEventSettingRepository _eventSettingRepository;

        public EventTemplateService(
            // IAttendanceClassificationRepository attendanceClassificationRepository,
            IActivityRepository activityRepository,
            ICompetitionTemplateService competitionTemplateService,
            IEmailTemplateService emailTemplateService,
            IEventSettingRepository eventSettingRepository
            )
        {
            //_attendanceClassificationRepository = attendanceClassificationRepository;
            _activityRepository = activityRepository;
            _competitionTemplateService = competitionTemplateService;
            _emailTemplateService = emailTemplateService;
            _eventSettingRepository = eventSettingRepository;
        }

        public async Task ApplyTemplate(Event myEvent, EventTemplateType templateType)
        {
            switch (templateType)
            {
                case EventTemplateType.Basic:
                    await ApplyBasicTemplate(myEvent);
                    break;
                case EventTemplateType.StandardDert:
                    await ApplyStandardDertTemplate(myEvent);
                    break;
                case EventTemplateType.ClassicDert:
                    await ApplyClassicDertTemplate(myEvent);
                    break;
                case EventTemplateType.Blank:
                    /* There are no additions on the blank template this is used when the event is marked as unconfigured*/
                    break;

            }
        }

        /// <summary>
        /// Basic Template applies a single ticket type and the minimal settings to allow the system to run.
        /// </summary>
        /// <param name="myEvent"></param>
        /// <returns></returns>
        private async Task ApplyBasicTemplate(Event myEvent)
        {
            //AttendanceClassification attendingClassification = new AttendanceClassification();
            //attendingClassification.EventId = myEvent.Id;
            //attendingClassification.ClassificationName = "Attending";
            //attendingClassification.ClassificationPrice = 0;
            //attendingClassification.IsDefault = true;
            //attendingClassification.AccessToken = null;
            //attendingClassification = await _attendanceClassificationRepository.Add(attendingClassification);

            Activity activity = new Activity();
            activity.EventId = myEvent.Id;
            activity.Title = "Attending";
            activity.Price = 0;
            activity.IsDefault = true;
            activity = await _activityRepository.Add(activity);

            EventSetting emailFromAddress = new EventSetting();
            emailFromAddress.EventId = myEvent.Id;
            emailFromAddress.Ref = EventSettingType.EMAIL_FROM_ADDRESS.ToString();
            emailFromAddress.Name = "Emails From Address";
            emailFromAddress.Value = "info@myevent.co.uk";
            emailFromAddress.AccessToken = null;
            emailFromAddress = await _eventSettingRepository.Add(emailFromAddress);

            EventSetting emailFromName = new EventSetting();
            emailFromName.EventId = myEvent.Id;
            emailFromName.Ref = EventSettingType.EMAIL_FROM_NAME.ToString();
            emailFromName.Name = "Emails From Name";
            emailFromName.Value = myEvent.Name;
            emailFromName.AccessToken = null;
            emailFromName = await _eventSettingRepository.Add(emailFromName);

            EventSetting emailBcc1 = new EventSetting();
            emailBcc1.EventId = myEvent.Id;
            emailBcc1.Ref = EventSettingType.EMAIL_BCC1.ToString();
            emailBcc1.Name = "Email Bcc";
            emailBcc1.Value = "copy1@myevent.co.uk";
            emailBcc1.AccessToken = null;
            emailBcc1 = await _eventSettingRepository.Add(emailBcc1);

            await _emailTemplateService.CreateRegistrationSubmissionDefault(myEvent.Id);
            await _emailTemplateService.CreateRegistrationConfirmationDefault(myEvent.Id);
        }

        private async Task ApplyStandardDertTemplate(Event myEvent)
        {
            //AttendanceClassification classification1 = new AttendanceClassification();
            //classification1.EventId = myEvent.Id;
            //classification1.ClassificationName = "Competitor With Accomodation";
            //classification1.ClassificationPrice = 0;
            //classification1.IsDefault = true;
            //classification1.AccessToken = null;
            //classification1 = await _attendanceClassificationRepository.Add(classification1);

            //AttendanceClassification classification2 = new AttendanceClassification();
            //classification2.EventId = myEvent.Id;
            //classification2.ClassificationName = "Consession";
            //classification2.ClassificationPrice = 0;
            //classification2.IsDefault = false;
            //classification2.AccessToken = null;
            //classification2 = await _attendanceClassificationRepository.Add(classification2);

            //AttendanceClassification classification3 = new AttendanceClassification();
            //classification3.EventId = myEvent.Id;
            //classification3.ClassificationName = "Spectator";
            //classification3.ClassificationPrice = 0;
            //classification3.IsDefault = false;
            //classification3.AccessToken = null;
            //classification3 = await _attendanceClassificationRepository.Add(classification3);

            EventSetting emailFromAddress = new EventSetting();
            emailFromAddress.EventId = myEvent.Id;
            emailFromAddress.Ref = EventSettingType.EMAIL_FROM_ADDRESS.ToString();
            emailFromAddress.Name = "Emails From Address";
            emailFromAddress.Value = "info@myevent.co.uk";
            emailFromAddress.AccessToken = null;
            emailFromAddress = await _eventSettingRepository.Add(emailFromAddress);

            EventSetting emailFromName = new EventSetting();
            emailFromName.EventId = myEvent.Id;
            emailFromName.Ref = EventSettingType.EMAIL_FROM_NAME.ToString();
            emailFromName.Name = "Emails From Name";
            emailFromName.Value = myEvent.Name;
            emailFromName.AccessToken = null;
            emailFromName = await _eventSettingRepository.Add(emailFromName);

            EventSetting emailBcc1 = new EventSetting();
            emailBcc1.EventId = myEvent.Id;
            emailBcc1.Ref = EventSettingType.EMAIL_BCC1.ToString();
            emailBcc1.Name = "Email Bcc";
            emailBcc1.Value = "copy1@myevent.co.uk";
            emailBcc1.AccessToken = null;
            emailBcc1 = await _eventSettingRepository.Add(emailBcc1);

            await _emailTemplateService.CreateRegistrationSubmissionDefault(myEvent.Id);
            await _emailTemplateService.CreateRegistrationConfirmationDefault(myEvent.Id);
            await _emailTemplateService.CreateEventCancellationDefault(myEvent.Id);

            // Apply the template and then pull the event settings and apply to the event
            List<EventSetting> competitionSettings = await _competitionTemplateService.ApplySwordDancingTemplate(myEvent.Id);
            foreach (EventSetting eventSetting in competitionSettings)
            {
                await _eventSettingRepository.Add(eventSetting);
            }

            //
        }

        private async Task ApplyClassicDertTemplate(Event myEvent)
        {
            EventSetting emailFromAddress = new EventSetting();
            emailFromAddress.EventId = myEvent.Id;
            emailFromAddress.Ref = EventSettingType.EMAIL_FROM_ADDRESS.ToString();
            emailFromAddress.Name = "Emails From Address";
            emailFromAddress.Value = "info@myevent.co.uk";
            emailFromAddress.AccessToken = null;
            emailFromAddress = await _eventSettingRepository.Add(emailFromAddress);

            EventSetting emailFromName = new EventSetting();
            emailFromName.EventId = myEvent.Id;
            emailFromName.Ref = EventSettingType.EMAIL_FROM_NAME.ToString();
            emailFromName.Name = "Emails From Name";
            emailFromName.Value = myEvent.Name;
            emailFromName.AccessToken = null;
            emailFromName = await _eventSettingRepository.Add(emailFromName);

            EventSetting emailBcc1 = new EventSetting();
            emailBcc1.EventId = myEvent.Id;
            emailBcc1.Ref = EventSettingType.EMAIL_BCC1.ToString();
            emailBcc1.Name = "Email Bcc";
            emailBcc1.Value = "copy1@myevent.co.uk";
            emailBcc1.AccessToken = null;
            emailBcc1 = await _eventSettingRepository.Add(emailBcc1);

            await _emailTemplateService.CreateRegistrationSubmissionDefault(myEvent.Id);
            await _emailTemplateService.CreateRegistrationConfirmationDefault(myEvent.Id);
            await _emailTemplateService.CreateEventCancellationDefault(myEvent.Id);

            // Apply the template and then pull the event settings and apply to the event
            List<EventSetting> competitionSettings = await _competitionTemplateService.ApplyClassicSwordDancingTemplate(myEvent.Id);
            foreach (EventSetting eventSetting in competitionSettings)
            {
                await _eventSettingRepository.Add(eventSetting);
            }
        }

    }
}
