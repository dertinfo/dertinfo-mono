using DertInfo.Models.Emails;
using DertInfo.Services.Entity.EmailTemplates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IEmailConstructionService<T> where T : EmailBase
    {
        Task<string> BuildEmailBody(T emailSendSubmission);
    }

    public class EmailConstructionService<T> where T : EmailBase
    {
        IEmailTemplateService _emailTemplateService;

        public EmailConstructionService(IEmailTemplateService emailTemplateService)
        {
            _emailTemplateService = emailTemplateService;
        }


        public async Task<string> BuildEmailBody(T emailSendSubmission)
        {
            // Load the template
            var template = await this._emailTemplateService.FindById(emailSendSubmission.EmailTemplateId);

            if(!emailSendSubmission.IsTemplateValid(template.TemplateRef)) { throw new InvalidOperationException("Data Submitted Does Not Match The Template"); }

            // Deal with the subjects. If the passed subject starts with @: then user this after the template subject
            if (emailSendSubmission.Subject == string.Empty) { emailSendSubmission.Subject = template.Subject; }
            if (emailSendSubmission.Subject.StartsWith("@:")) { emailSendSubmission.Subject = template.Subject + " - " + emailSendSubmission.Subject.Substring(2); }

            var replacements = emailSendSubmission.ExtractReplacements();
            var delimiter = "##";

            var mergedTemplateBody = this.MergeTemplate(template.Body, delimiter, replacements);

            return mergedTemplateBody;
        }

        private string MergeTemplate(string template, string delimiterWrapper, List<KeyValuePair<string, string>> replacements)
        {
            foreach (KeyValuePair<string, string> kvp in replacements)
            {
                string replacementTarget = delimiterWrapper + kvp.Key.ToUpper() + delimiterWrapper;
                string replacementValue = kvp.Value;
                template = template.Replace(replacementTarget, replacementValue);
            }

            //Clear all new line chars
            template = template.Replace(Environment.NewLine, string.Empty);

            return template;
        }

        protected virtual List<KeyValuePair<string, string>> BuildReplacementDictionary(T emailSendSubmission)
        {
            var replacements = new List<KeyValuePair<string, string>>();

            // example: _emailObject.AddReplacement("GROUPNAME", _group.GroupName);

            return replacements;
        }

        protected virtual bool isTemplateValid(string templateRef)
        {
            return false;
        }
    }
}
