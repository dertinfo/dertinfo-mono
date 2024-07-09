using AutoMapper;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.DataTransferObject.Emails;
using DertInfo.Models.Emails;
using DertInfo.Services;
using DertInfo.Services.Entity.EmailTemplates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// SHOULD BE SEPERATE API 
/// 
/// This Api will take details of an email submission and construct the email according to the specified endpoint called. 
/// The template will be determined by the event which needs to be supplied on the submission.
/// However this is the limit of the interaction between the main system and this.
/// 
/// Get the template / Apply the data.
/// 
/// DO NOT POLLUTE THIS CONTROLLER WITH DERT STUFF.
/// 
/// 
/// Security Considerations: 
/// Any authorised user can hit this enpoint which would send an email. 
/// The could send the email provided that the template matches and they could send the information to an alternative person.
/// However every email sent would match the template. Therefore the risk is reduced here. 
/// Email submissions that do not match the template are not sent. 
/// Email submissions where the template does not match the event are not submitted.
/// </summary>
namespace DertInfo.Api.Controllers
{
    /// <summary>
    /// SendEmailController - used for sending specific emails to users.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    public class SendEmailController : AuthController
    {
        private string _errorMessage = string.Empty;

        IMapper _mapper;
        IEmailSendingService _emailSendingService;
        IEmailTemplateService _emailTemplateService;
        

        public SendEmailController(
            IMapper mapper,
            IEmailSendingService emailSendingService,
            IEmailTemplateService emailTemplateService,
            IDertInfoUser user
            ) : base(user)
        {
            _mapper = mapper;
            _emailSendingService = emailSendingService;
            _emailTemplateService = emailTemplateService;
        }

        [Obsolete("This method is no longer used. Was formally used to submit from MVC Web")]
        [Route("send-group-registration-submission")]
        [HttpPost]
        public async Task<IActionResult> SendGroupRegistrationSubmission([FromBody] GroupRegistationSubmissionEmailData groupRegistrationSubmission)
        {
            if (!ValidSubmission(groupRegistrationSubmission)) { return BadRequest(this._errorMessage); }

            var emailConstructionService = new EmailConstructionService<GroupRegistationSubmissionEmailData>(this._emailTemplateService);
            // Get the required template
            var emailBody = await emailConstructionService.BuildEmailBody(groupRegistrationSubmission);

            await this._emailSendingService.SendEmail(groupRegistrationSubmission, emailBody);

            // Return the preview
            return Ok(new EmailPreviewDto() { HtmlBody = emailBody });
        }

        [Obsolete("This method is no longer used. Was formally used to submit from MVC Web")]
        [Route("send-group-registration-confirmation")]
        [HttpPost]
        public async Task<IActionResult> SendGroupRegistrationConfirmation([FromBody] GroupRegistationConfirmationEmailData groupRegistrationConfirmation)
        {
            if (!ValidSubmission(groupRegistrationConfirmation)) { return BadRequest(this._errorMessage); }

            var emailConstructionService = new EmailConstructionService<GroupRegistationConfirmationEmailData>(this._emailTemplateService);
            // Get the required template
            var emailBody = await emailConstructionService.BuildEmailBody(groupRegistrationConfirmation);

            await this._emailSendingService.SendEmail(groupRegistrationConfirmation, emailBody);

            // Return the preview
            return Ok(new EmailPreviewDto() { HtmlBody = emailBody });
        }


        // note - this has been removed as it will not work with the defined authorisation policy. This policy requires the groupId to be available on the route. 
        //[Route("send-gdpr-notification")]
        //[Authorize(Policy = "GroupAdministratorOnlyPolicy")]
        //public async Task<IActionResult> SendGdprNotification([FromBody] GdprNotification gdprNotification)
        //{
        //    if (!ValidSubmission(gdprNotification)) { return BadRequest(this._errorMessage); }

        //    var emailConstructionService = new EmailConstructionService<GdprNotification>(this._emailTemplateService);
        //    // Get the required template
        //    var emailBody = await emailConstructionService.BuildEmailBody(gdprNotification);

        //    await this._emailSendingService.SendEmail(gdprNotification, emailBody);

        //    // Return the preview
        //    return Ok(emailBody);
        //}

        [Obsolete("This method is no longer used. Was formally used to submit from MVC Web")]
        [Route("preview-group-registration-submission")]
        [HttpPost]
        public async Task<IActionResult> PreviewGroupRegistrationSubmission([FromBody] GroupRegistationSubmissionEmailData groupRegistrationSubmission)
        {
            if (!ValidSubmission(groupRegistrationSubmission)) { return BadRequest(this._errorMessage); }

            var emailConstructionService = new EmailConstructionService<GroupRegistationSubmissionEmailData>(this._emailTemplateService);
            // Get the required template
            var emailBody = await emailConstructionService.BuildEmailBody(groupRegistrationSubmission);

            // Return the preview
            return Ok(new EmailPreviewDto() { HtmlBody = emailBody });
        }

        [Route("preview-group-registration-confirmation")]
        [HttpPost]
        public async Task<IActionResult> PreviewGroupRegistrationConfirmation([FromBody] GroupRegistationConfirmationEmailData groupRegistrationConfirmation)
        {
            if (!ValidSubmission(groupRegistrationConfirmation)) { return BadRequest(this._errorMessage); }

            var emailConstructionService = new EmailConstructionService<GroupRegistationConfirmationEmailData>(this._emailTemplateService);
            // Get the required template
            var emailBody = await emailConstructionService.BuildEmailBody(groupRegistrationConfirmation);

            // Return the preview
            return Ok(new EmailPreviewDto() { HtmlBody = emailBody });
        }

        [Obsolete("This method is no longer used. Was formally used to submit from MVC Web")]
        [Route("preview-gdpr-notification")]
        [HttpPost]
        public async Task<IActionResult> PreviewGdprNotification([FromBody] GdprNotification gdprNotification)
        {
            if (!ValidSubmission(gdprNotification)) { return BadRequest(this._errorMessage); }

            var emailConstructionService = new EmailConstructionService<GdprNotification>(this._emailTemplateService);
            // Get the required template
            var emailBody = await emailConstructionService.BuildEmailBody(gdprNotification);

            // Return the preview
            return Ok(new EmailPreviewDto() { HtmlBody = emailBody });
        }


        private bool ValidSubmission(EmailBase emailSubmission)
        {
            if (emailSubmission.ToAddresses.Length == 0)
            {
                this._errorMessage = "must supply to address.";
                return false;
            }
            if (emailSubmission.FromAddress == string.Empty)
            {
                this._errorMessage = "must supply from address.";
                return false;
            }
            if (emailSubmission.Subject == string.Empty)
            {
                this._errorMessage = "must supply subject";
                return false;
            }
            if (emailSubmission.EmailTemplateId == 0)
            {
                this._errorMessage = "email cannot be generated without a template supplied";
                return false;
            }

            return true;
        }
    }
}
