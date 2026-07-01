using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IInvoiceRepository : IRepository<Invoice, int>
    {
        Task<Invoice> GetLatestForRegistration(int registrationId);
        Task MarkDeletedByRegistration(int registrationId);
        Task<IEnumerable<Invoice>> GetForGroupWithEventInfo(int groupId);
        Task<IEnumerable<Invoice>> GetForEventWithGroupInfo(int eventId);
        Task<Invoice> GetForAuthorization(int invoiceId);
        Task<IEnumerable<Invoice>> ListAllForRegistration(int registrationId);
    }

    public class InvoiceRepository : BaseRepository<Invoice, int, DertInfoContext>, IInvoiceRepository
    {
        public InvoiceRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {

        }

        public async Task<Invoice> GetForAuthorization(int invoiceId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Invoices
                    .Include(i => i.Registration)
                    .Where(i => i.Id == invoiceId);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<IEnumerable<Invoice>> GetForGroupWithEventInfo(int groupId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Invoices
                    .Include(i => i.Registration).ThenInclude(r => r.Event).ThenInclude(e => e.EventImages).ThenInclude(ei => ei.Image)
                    .Where(i => i.Registration.GroupId == groupId);
 
                return query.OrderByDescending(inv => inv.DateCreated).ToList();
            });

            return await task;
        }

        public async Task<IEnumerable<Invoice>> GetForEventWithGroupInfo(int eventId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Invoices
                    .Include(i => i.Registration).ThenInclude(r => r.Group).ThenInclude(e => e.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(ei => ei.Image)
                    .Where(i => i.Registration.EventId == eventId);

                return query.OrderByDescending(inv => inv.DateCreated).ToList();
            });

            return await task;
        }

        public async Task<Invoice> GetLatestForRegistration(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Invoices.Where(inv => inv.RegistrationId == registrationId);

                return query.OrderByDescending(inv => inv.DateCreated).FirstOrDefault();
            });

            return await task;
        }

        public async Task<IEnumerable<Invoice>> ListAllForRegistration(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Invoices
                    .Include(i => i.Registration).ThenInclude(r => r.Group).ThenInclude(e => e.GroupImages.Where(gi => gi.IsPrimary)).ThenInclude(ei => ei.Image)
                    .Where(inv => inv.RegistrationId == registrationId);

                return query.IgnoreQueryFilters().OrderByDescending(inv => inv.DateCreated);
            });

            return await task;
        }

        /// <summary>
        /// Used to mark all previous invoices for a registration as deleted before adding a new one
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        public async Task MarkDeletedByRegistration(int registrationId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Invoices.Where(inv => inv.RegistrationId == registrationId);

                foreach (var invoice in query.ToList())
                {
                    invoice.IsDeleted = true;
                }

                _context.SaveChanges();
            });

            await task;
        }

        
    }
}
