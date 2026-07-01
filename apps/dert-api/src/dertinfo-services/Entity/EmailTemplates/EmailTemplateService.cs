using DertInfo.Models.Database;
using DertInfo.Models.System;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using DertInfo.Services.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.EmailTemplates
{
    public interface IEmailTemplateService
    {
        Task<EmailTemplate> FindById(int emailTemplateId);
        Task<IEnumerable<EmailTemplate>> ListByEvent(int eventId);
        Task<int> CreateRegistrationSubmissionDefault(int eventId);
        Task<int> CreateRegistrationConfirmationDefault(int eventId);
        Task<int> CreateEventCancellationDefault(int eventId);
        Task<EmailTemplate> UpdateEmailTemplate(EmailTemplate myEmailTemplate);
    }

    public class EmailTemplateService: IEmailTemplateService
    {
        IEmailTemplateRepository _emailTemplateRepository;

        public EmailTemplateService(IEmailTemplateRepository emailTemplateRepository) {
            _emailTemplateRepository = emailTemplateRepository;
        }

        public async Task<IEnumerable<EmailTemplate>> ListByEvent(int eventId)
        {
            var emailTemplates = await _emailTemplateRepository.Find(et => et.EventId == eventId);
            return emailTemplates;
        }

        public async Task<EmailTemplate> FindById(int emailTemplateId)
        {
            var emailTemplate = await _emailTemplateRepository.GetById(emailTemplateId);
            return emailTemplate;
        }

        public async Task<int> CreateRegistrationSubmissionDefault(int eventId)
        {
            EmailTemplate registrationSubmissionTemplate = new EmailTemplate();
            registrationSubmissionTemplate.EventId = eventId;
            registrationSubmissionTemplate.TemplateRef = Enum.GetName(typeof(EmailTemplateType), EmailTemplateType.REGISTRATION_SUBMIT);
            registrationSubmissionTemplate.TemplateName = "Registration Submission";
            registrationSubmissionTemplate.Subject = "Registration Received";
            registrationSubmissionTemplate.AccessToken = null;
            registrationSubmissionTemplate.Body = DertInfo.Services.Properties.Resources.EmailTemplate_Registration_Submit_Default;

            registrationSubmissionTemplate = await _emailTemplateRepository.Add(registrationSubmissionTemplate);

            return registrationSubmissionTemplate.Id;
        }

        public async Task<int> CreateRegistrationConfirmationDefault(int eventId)
        {
            EmailTemplate registrationConfirmationTemplate = new EmailTemplate();
            registrationConfirmationTemplate.EventId = eventId;
            registrationConfirmationTemplate.TemplateRef = Enum.GetName(typeof(EmailTemplateType), EmailTemplateType.REGISTRATION_CONFIRM);
            registrationConfirmationTemplate.TemplateName = "Registration Confirmation";
            registrationConfirmationTemplate.Subject = "Registration Confirmed";
            registrationConfirmationTemplate.AccessToken = null;
            registrationConfirmationTemplate.Body = DertInfo.Services.Properties.Resources.EmailTemplate_Registration_Confirm_Default;

            registrationConfirmationTemplate = await _emailTemplateRepository.Add(registrationConfirmationTemplate);

            return registrationConfirmationTemplate.Id;
        }

        public async Task<int> CreateEventCancellationDefault(int eventId)
        {
            EmailTemplate eventCancellationTemplate = new EmailTemplate();
            eventCancellationTemplate.EventId = eventId;
            eventCancellationTemplate.TemplateRef = Enum.GetName(typeof(EmailTemplateType), EmailTemplateType.EVENT_CANCELLATION);
            eventCancellationTemplate.TemplateName = "Event Cancelled";
            eventCancellationTemplate.Subject = "Event Cancelled";
            eventCancellationTemplate.AccessToken = null;
            eventCancellationTemplate.Body = DertInfo.Services.Properties.Resources.EmailTemplate_Event_Cancel_Default;

            eventCancellationTemplate = await _emailTemplateRepository.Add(eventCancellationTemplate);

            return eventCancellationTemplate.Id;
        }

        public async Task<EmailTemplate> UpdateEmailTemplate(EmailTemplate myEmailTemplate)
        {
            var originalEmailTemplate = await _emailTemplateRepository.GetById(myEmailTemplate.Id);

            if (originalEmailTemplate == null) { throw new InvalidOperationException("Email Template Could Not Be Found"); }

            if (originalEmailTemplate.EventId != myEmailTemplate.EventId) { throw new InvalidOperationException("The template Specified is not for the event specified"); } 

            if (originalEmailTemplate.TemplateName != myEmailTemplate.TemplateName)
            {
                originalEmailTemplate.TemplateName = myEmailTemplate.TemplateName;
            }

            if (originalEmailTemplate.Subject != myEmailTemplate.Subject)
            {
                originalEmailTemplate.Subject = myEmailTemplate.Subject;
            }

            if (originalEmailTemplate.Body != myEmailTemplate.Body)
            {
                originalEmailTemplate.Body = myEmailTemplate.Body;
            }

            await _emailTemplateRepository.Update(originalEmailTemplate);

            return originalEmailTemplate;
        }
    }
}
