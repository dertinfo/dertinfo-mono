using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DertInfo.Services
{
    public interface IInvoiceService
    {
        Task<decimal> GetLatestInvoicedPriceForRegistration(int registrationId);
        Task<Invoice> CreateInvoiceForRegistration(int id);
        Task<IEnumerable<Invoice>> GetInvoicesForGroup(int groupId);
        Task<IEnumerable<EventInvoiceDO>> GetInvoicesForEvent(int eventId);
        Task<Invoice> GetForAuthorization(int invoiceId);
        Task SetInvoicePaidStatus(int id, bool hasPaid);
        Task<IEnumerable<Invoice>> GetAllByRegistrationId(int id);
    }

    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IPricingService _pricingService;
        private readonly IRegistrationRepository _registrationRepo;

        public InvoiceService(
            IInvoiceRepository invoiceRepo,
            IPricingService pricingService,
            IRegistrationRepository registrationRepo
            )
        {
            this._invoiceRepo = invoiceRepo;
            this._pricingService = pricingService;
            this._registrationRepo = registrationRepo;
        }

        public Task<Invoice> GetForAuthorization(int invoiceId)
        {
            return this._invoiceRepo.GetForAuthorization(invoiceId);
        }

        public async Task<Invoice> CreateInvoiceForRegistration(int registrationId)
        {
            var registration = await this._registrationRepo.GetForEmail(registrationId);

            try
            {
                var invoiceCode = this.GenerateInvoiceCode(registration.Group.GroupName);

                Invoice invoice = new Invoice
                {
                    InvoiceCode = invoiceCode,
                    HasPaid = false,
                    InvoiceToEmail = registration.Group.PrimaryContactEmail,
                    InvoiceTotal = await this._pricingService.GetCurrentPriceForRegistration(registrationId),
                    InvoiceToName = registration.Group.PrimaryContactName,
                    InvoiceToTeamName = registration.Group.GroupName,
                    RegistrationId = registrationId,
                    InvoiceEntryNotes = this.BuildInvoiceEntryNotes(registration),
                    InvoiceLineItemNotes = this.BuildInvoiceEntryLineItemNotes(registration)
                };

                await this._invoiceRepo.MarkDeletedByRegistration(registrationId);

                return await this._invoiceRepo.Add(invoice);
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create invoice for registration:", ex);
            }
        }

        

        public async Task<decimal> GetLatestInvoicedPriceForRegistration(int registrationId)
        {
            var latestInvoice = await this._invoiceRepo.GetLatestForRegistration(registrationId);

            if (latestInvoice != null)
            {
                return latestInvoice.InvoiceTotal;
            }
            else
            {
                return 0;
            }
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesForGroup(int groupId)
        {
            return await this._invoiceRepo.GetForGroupWithEventInfo(groupId);
        }

        public async Task<IEnumerable<EventInvoiceDO>> GetInvoicesForEvent(int eventId)
        {
            var invoices = await this._invoiceRepo.GetForEventWithGroupInfo(eventId);

            var invoiceDOs = new List<EventInvoiceDO>();

            foreach (var invoice in invoices)
            {
                EventInvoiceDO invoiceDO = new EventInvoiceDO(invoice);
                invoiceDO.CurrentRegistrationTotal = await this._pricingService.GetCurrentPriceForRegistration(invoiceDO.RegistrationId);
                invoiceDOs.Add(invoiceDO);
            }

            return invoiceDOs;
        }

        public async Task<IEnumerable<Invoice>> GetAllByRegistrationId(int registrationId)
        {
            return await this._invoiceRepo.ListAllForRegistration(registrationId);
        }

        public async Task SetInvoicePaidStatus(int id, bool hasPaid)
        {
            var invoice = await this._invoiceRepo.GetById(id);

            invoice.HasPaid = hasPaid;

            await this._invoiceRepo.Update(invoice);
        }

        #region Stolen From The Email Service

        private string BuildInvoiceEntryLineItemNotes(Registration registration)
        {
            Dictionary<string, KeyValuePair<int, decimal>> collatedIndividualLineItems = new Dictionary<string, KeyValuePair<int, decimal>>();
            Dictionary<string, decimal> individualActivityUnitPrice = new Dictionary<string, decimal>();
            decimal individualTotal = 0;

            Dictionary<string, KeyValuePair<int, decimal>> collatedTeamLineItems = new Dictionary<string, KeyValuePair<int, decimal>>();
            Dictionary<string, decimal> teamActivityUnitPrice = new Dictionary<string, decimal>();
            decimal teamTotal = 0;


            this.CollateIndividualLines(registration.MemberAttendances, out collatedIndividualLineItems, out individualActivityUnitPrice, out individualTotal);
            this.CollateTeamLines(registration.TeamAttendances, out collatedTeamLineItems, out teamActivityUnitPrice, out teamTotal);


            var flattenIndividualLines = collatedIndividualLineItems.Select(x => new FlatLine {
                Key = x.Key,
                SubKey = x.Value.Key,
                Value = x.Value.Value
            });

            var flattenIndividualPrices = individualActivityUnitPrice.Select(x => new FlatPrice
            {
                Key = x.Key,
                Value = x.Value
            });

            var flattenTeamLines = collatedTeamLineItems.Select(x => new FlatLine
            {
                Key = x.Key,
                SubKey = x.Value.Key,
                Value = x.Value.Value
            });

            var flattenTeamPrices = teamActivityUnitPrice.Select(x => new FlatPrice
            {
                Key = x.Key,
                Value = x.Value
            });

            var linesDetailStorage = new LinesDetailStorage();
            linesDetailStorage.collatedIndividualLineItems = flattenIndividualLines.ToList();
            linesDetailStorage.individualActivityUnitPrice = flattenIndividualPrices.ToList();
            linesDetailStorage.individualTotal = individualTotal;
            linesDetailStorage.collatedTeamLineItems = flattenTeamLines.ToList();
            linesDetailStorage.teamActivityUnitPrice = flattenTeamPrices.ToList();
            linesDetailStorage.teamTotal = teamTotal;

            return linesDetailStorage.ToXML();

        }

        private string BuildInvoiceEntryNotes(Registration registration)
        {
            var notesDetailStorage = new NotesDetailStorage();
            notesDetailStorage.GroupName = registration.Group.GroupName;
            notesDetailStorage.EventName = registration.Event.Name;
            notesDetailStorage.ContactName = registration.Group.PrimaryContactName;

            return notesDetailStorage.ToXML();
        }

        public void CollateIndividualLines(ICollection<MemberAttendance> memberAttendanceLineItems, out Dictionary<string, KeyValuePair<int, decimal>> collatedIndividualLineItems, out Dictionary<string, decimal> individualActivityUnitPrice, out decimal linesTotal)
        {
            linesTotal = 0;

            collatedIndividualLineItems = new Dictionary<string, KeyValuePair<int, decimal>>();
            individualActivityUnitPrice = new Dictionary<string, decimal>();

            foreach (var individualLineItem in memberAttendanceLineItems)
            {
                foreach (var memberActivity in individualLineItem.MemberActivities)
                {
                    var activity = memberActivity.Activity;

                    var activityPrice = activity.Price;

                    if (collatedIndividualLineItems.ContainsKey(activity.Title))
                    {
                        var dictionaryEntry = collatedIndividualLineItems[activity.Title];
                        var instanceCount = dictionaryEntry.Key;
                        var instanceTotal = dictionaryEntry.Value;

                        collatedIndividualLineItems[activity.Title] = new KeyValuePair<int, decimal>(++instanceCount, instanceTotal + activityPrice);

                        linesTotal += activityPrice;
                    }
                    else
                    {
                        individualActivityUnitPrice[activity.Title] = activityPrice;
                        collatedIndividualLineItems[activity.Title] = new KeyValuePair<int, decimal>(1, activityPrice);

                        linesTotal += activityPrice;
                    }
                }
            }
        }

        public void CollateTeamLines(ICollection<TeamAttendance> teamAttendances, out Dictionary<string, KeyValuePair<int, decimal>> collatedTeamActivityLineItems, out Dictionary<string, decimal> teamActivityUnitPrice, out decimal linesTotal)
        {
            linesTotal = 0;

            collatedTeamActivityLineItems = new Dictionary<string, KeyValuePair<int, decimal>>();
            teamActivityUnitPrice = new Dictionary<string, decimal>();

            foreach (var teamLineItem in teamAttendances)
            {
                foreach (var teamAttendanceActivity in teamLineItem.TeamActivities)
                {
                    var activity = teamAttendanceActivity.Activity;

                    var activityPrice = activity.Price;

                    if (collatedTeamActivityLineItems.ContainsKey(activity.Title))
                    {
                        var dictionaryEntry = collatedTeamActivityLineItems[activity.Title];
                        var instanceCount = dictionaryEntry.Key;
                        var instanceTotal = dictionaryEntry.Value;

                        collatedTeamActivityLineItems[activity.Title] = new KeyValuePair<int, decimal>(++instanceCount, instanceTotal + activityPrice);

                        linesTotal += activityPrice;
                    }
                    else
                    {
                        teamActivityUnitPrice[activity.Title] = activityPrice;
                        collatedTeamActivityLineItems[activity.Title] = new KeyValuePair<int, decimal>(1, activityPrice);

                        linesTotal += activityPrice;
                    }
                }
            }
        }

        #endregion

        private string GenerateInvoiceCode(string groupName)
        {
            var now = DateTime.Now;
            var invoiceDateStamp = now.Year.ToString() + now.Month.ToString() + now.Day.ToString();
            var prefix = groupName.Length > 3 ? groupName.Substring(0, 3) : "INV";

            return prefix + invoiceDateStamp;
        }
    }

    public class LinesDetailStorage
    {
        public List<FlatLine> collatedIndividualLineItems;
        public List<FlatPrice> individualActivityUnitPrice;
        public decimal individualTotal;

        public List<FlatLine> collatedTeamLineItems;
        public List<FlatPrice> teamActivityUnitPrice;
        public decimal teamTotal;

        public string ToXML()
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        public static LinesDetailStorage LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(LinesDetailStorage));
            return serializer.Deserialize(stringReader) as LinesDetailStorage;
        }
    }

    public class NotesDetailStorage
    {
        public string GroupName;
        public string ContactName;
        public string EventName;

        public string ToXML()
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        public static NotesDetailStorage LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(NotesDetailStorage));
            return serializer.Deserialize(stringReader) as NotesDetailStorage;
        }
    }

    public class FlatLine
    {
        public string Key;
        public int SubKey;
        public decimal Value;
    }

    public class FlatPrice
    {
        public string Key;
        public decimal Value;
    }

    
}
