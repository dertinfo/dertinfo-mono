using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Emails;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IEmailSendingService
    {
        Task SendEmail(EmailBase emailBase, string emailbody);
    }

    public class EmailSendingService : IEmailSendingService
    {
        private string _sendGridApiKey;
        private bool _sendGridEnabled;

        public EmailSendingService(IDertInfoConfiguration configuration)
        {
            _sendGridApiKey = configuration.SendGrid_ApiKey;
            _sendGridEnabled = configuration.SendGrid_Enabled;
        }

        public async Task SendEmail(EmailBase emailBase, string htmlBody) {

            if (this._sendGridEnabled)
            {
                var apiKey = this._sendGridApiKey;
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(emailBase.FromAddress, emailBase.FromName);


                List<EmailAddress> tos = new List<EmailAddress>();
                emailBase.ToAddresses = emailBase.ToAddresses ?? new string[] { };
                foreach (string toAddress in emailBase.ToAddresses)
                {
                    tos.Add(new EmailAddress(toAddress));
                }

                emailBase.CcAddresses = emailBase.CcAddresses ?? new string[] { };
                foreach (string ccAddress in emailBase.CcAddresses)
                {
                    tos.Add(new EmailAddress(ccAddress));
                }

                emailBase.BccAddresses = emailBase.BccAddresses ?? new string[] { };
                foreach (string bccAddress in emailBase.BccAddresses)
                {
                    tos.Add(new EmailAddress(bccAddress));
                }


                var subject = emailBase.Subject;
                var htmlContent = htmlBody;

                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, null, htmlContent, false);

                var response = await client.SendEmailAsync(msg);
            }
        }
    }
}
