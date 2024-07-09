using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;

namespace DertInfo.Util.Email
{
    public class EmailObject
    {
        internal string FromAddress { get; set; }
        internal List<string> ToAddresses { get; set; }
        internal List<string> CCAddresses { get; set; }
        internal List<string> BCCAddresses { get; set; }
        internal string Subject { get; set; }
        internal string Template { get; set; }
        internal string TemplateDelimiterWrapper { get; set; }
        internal List<KeyValuePair<string, string>> Replacements { get; set; }
        internal List<KeyValuePair<string, Byte[]>> Attachments { get; set; }
        internal string SmtpServer { get; set; }
        internal string SmtpUsername { get; set; }
        internal string SmtpPassword { get; set; }
        internal bool IsGmail { get; set; }
        

        public EmailObject(string fromAddress, string toAddress, string subject, string template, string templateDelimiterWrapper, string smtpServer, string smtpUsername, string smtpPassword, bool isGmail)
        {
            this.ToAddresses = new List<string>();
            this.CCAddresses = new List<string>();
            this.BCCAddresses = new List<string>();
            this.Replacements = new List<KeyValuePair<string, string>>();
            this.Attachments = new List<KeyValuePair<string, Byte[]>>();

            //From
            if (fromAddress.Length > 0)
            {
                this.FromAddress = fromAddress;
            }
            else
            {
                throw new ArgumentException("From Address is Empty");
            }

            //To
            if (toAddress.Length > 0)
            {
                this.ToAddresses.Add(toAddress);
            }
            else
            {
                throw new ArgumentException("To Address is Empty");
            }

            //Subject
            if (subject.Length > 0)
            {
                this.Subject = subject;
            }
            else
            {
                throw new ArgumentException("Subject is Empty");
            }

            this.Template = template;
            this.TemplateDelimiterWrapper = templateDelimiterWrapper;

            this.IsGmail = isGmail;

            this.SmtpServer = smtpServer;
            this.SmtpUsername = smtpUsername;
            this.SmtpPassword = smtpPassword;

        }

        public void AddToAddress(string toAddress)
        { 
            if(toAddress.Length > 0)
            {
                this.ToAddresses.Add(toAddress);
            }
        }

        public void AddCcAddress(string ccAddress)
        { 
            if(ccAddress.Length > 0)
            {
                this.CCAddresses.Add(ccAddress);
            }
        }

        public void AddBccAddress(string bccAddress)
        { 
            if(bccAddress.Length > 0)
            {
                this.BCCAddresses.Add(bccAddress);
            }
        }

        public void AddReplacement(string key, string value)
        {
            if (key.Length > 0)
            {
                KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(key,value);
                this.Replacements.Add(keyValuePair);
            }
        }

        public void AddAttachement(string attachmentName, Byte[] attachmentFile)
        {
            if (attachmentName.Length > 0 && attachmentFile.Length > 0)
            {
                KeyValuePair<string, Byte[]> attachement = new KeyValuePair<string, byte[]>(attachmentName, attachmentFile);
                this.Attachments.Add(attachement);
            }
        }

        public string GetPreview()
        {
            return this.MergeTemplate(this.Template, this.TemplateDelimiterWrapper, this.Replacements);
        }

        public void SendEmail()
        {
            //Construct the message.
            MailMessage mailMessage = new MailMessage();

            //From
            if (this.FromAddress.Length > 0)
            {
                MailAddress fromAddress = new MailAddress(this.FromAddress);
                mailMessage.From = fromAddress;
            }

            //To
            if (this.ToAddresses.Count() == 0) { throw new ArgumentException("No To Address Set For Mail"); } //STOP
            foreach(string toAddress in this.ToAddresses)
            {
                if (toAddress.Length > 0)
                {
                    MailAddress mailAddress = new MailAddress(toAddress);
                    mailMessage.To.Add(mailAddress);
                }
            }

            //CC
            foreach (string ccAddress in this.CCAddresses)
            {
                MailAddress mailAddress = new MailAddress(ccAddress);
                mailMessage.CC.Add(mailAddress);
            }

            //Bcc
            foreach (string bccAddress in this.BCCAddresses)
            {
                MailAddress mailAddress = new MailAddress(bccAddress);
                mailMessage.Bcc.Add(mailAddress);
            }

            //Subject
            mailMessage.Subject = this.Subject;

            //Tempate & Replacements
            this.Template = this.MergeTemplate(this.Template,this.TemplateDelimiterWrapper,this.Replacements);
            mailMessage.Body = this.Template;

            //Attachements
            foreach (KeyValuePair<string,Byte[]> attachement in this.Attachments)
            {
                Attachment att = new Attachment(new MemoryStream(attachement.Value), attachement.Key);
                mailMessage.Attachments.Add(att);
            }

            mailMessage.IsBodyHtml = true;

            mailMessage.Priority = MailPriority.Normal;

            Send(mailMessage);
        }

        private void Send(MailMessage mailMessage)
        {
            if (this.IsGmail)
            {
                try
                {
                    SmtpClient smtpClient = new SmtpClient(this.SmtpServer);
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new System.Net.NetworkCredential(this.SmtpUsername, this.SmtpPassword);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = true;
                    smtpClient.Port = 587;
                    smtpClient.Timeout = 30000;

                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    //TODO: Handle mail sending error types.
                    throw ex;
                }
            }
            else
            {
                SmtpClient smtpClient = new SmtpClient(this.SmtpServer);
                smtpClient.Credentials = new System.Net.NetworkCredential(this.SmtpUsername, this.SmtpPassword);
                smtpClient.Send(mailMessage);
            }
        }

        private string MergeTemplate(string template, string delimiterWrapper, List<KeyValuePair<string,string>> replacements)
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

        

    }
}
