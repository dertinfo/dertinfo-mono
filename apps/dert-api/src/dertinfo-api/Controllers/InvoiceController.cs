using AutoMapper;
using DertInfo.Api.AuthorisationPolicies.ResourceBased;
using DertInfo.Api.Controllers.Base;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Models.DataTransferObject;
using DertInfo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace DertInfo.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class InvoiceController : ResourceAuthController
    {
        IMapper _mapper;
        IInvoiceService _invoiceService;

        public InvoiceController(
            IMapper mapper,
            IAuthorizationService authorizationService,
            IInvoiceService invoiceService,
            IDertInfoUser user
            ) : base(user, authorizationService)
        {
            _mapper = mapper;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Route("event/{eventId}")]
        [Authorize(Policy = "EventAdministratorOnlyPolicy")]
        public async Task<IActionResult> GetInvoices(int eventId)
        {
            base.ExtractUser(); //Fill the scoped injected IDertInfoUser

            var eventInvoices = await _invoiceService.GetInvoicesForEvent(eventId);

            // Perform Auto Map of simple fields
            List<EventInvoiceDto> eventInvoiceDtos = _mapper.Map<List<EventInvoiceDto>>(eventInvoices);

            foreach (var invoiceDto in eventInvoiceDtos)
            {
                try
                {
                    XmlDocument entryNotesXml = new XmlDocument();
                    entryNotesXml.LoadXml(invoiceDto.InvoiceEntryNotes);
                    string jsonTextEntryNotes = JsonConvert.SerializeXmlNode(entryNotesXml);
                    invoiceDto.InvoiceEntryNotes = jsonTextEntryNotes;

                    XmlDocument invoiceLinesXml = new XmlDocument();
                    invoiceLinesXml.LoadXml(invoiceDto.InvoiceLineItemNotes);
                    string jsonTextInvoiceLines = JsonConvert.SerializeXmlNode(invoiceLinesXml);
                    invoiceDto.InvoiceLineItemNotes = jsonTextInvoiceLines;

                    invoiceDto.HasStructuredNotes = true;
                }
                catch
                {
                    invoiceDto.HasStructuredNotes = false;
                }
            }

            return Ok(eventInvoiceDtos);
        }


        /// <summary>
        /// The passed invoice here must be one that is not deleted. Else 404 not found will be returned.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{invoiceId}/history")]
        public async Task<IActionResult> GetInvoiceHistory(int invoiceId)
        {
            var authorisationPolicy = InvoiceGetHistoryPolicy.PolicyName;

            base.ExtractUser(); // Fill the scoped injected IDertInfoUser

            Invoice invoice = await this._invoiceService.GetForAuthorization(invoiceId);

            if (invoice == null) return NotFound();

            if (await this.CheckAuthorisationPolicy(authorisationPolicy, invoice))
            {
                var allRegistrationInvoices = await this._invoiceService.GetAllByRegistrationId(invoice.RegistrationId);

                // Perform Auto Map of simple fields
                List<EventInvoiceDto> eventInvoiceDtos = _mapper.Map<List<EventInvoiceDto>>(allRegistrationInvoices);

                foreach (var invoiceDto in eventInvoiceDtos)
                {
                    try
                    {
                        XmlDocument entryNotesXml = new XmlDocument();
                        entryNotesXml.LoadXml(invoiceDto.InvoiceEntryNotes);
                        string jsonTextEntryNotes = JsonConvert.SerializeXmlNode(entryNotesXml);
                        invoiceDto.InvoiceEntryNotes = jsonTextEntryNotes;

                        XmlDocument invoiceLinesXml = new XmlDocument();
                        invoiceLinesXml.LoadXml(invoiceDto.InvoiceLineItemNotes);
                        string jsonTextInvoiceLines = JsonConvert.SerializeXmlNode(invoiceLinesXml);
                        invoiceDto.InvoiceLineItemNotes = jsonTextInvoiceLines;

                        invoiceDto.HasStructuredNotes = true;
                    }
                    catch
                    {
                        invoiceDto.HasStructuredNotes = false;
                    }
                }

                return Ok(eventInvoiceDtos);
            }
            else
            {
                return StatusCode(403, " Failed to meet requirements for " + authorisationPolicy);
            }
        }
    }
}
