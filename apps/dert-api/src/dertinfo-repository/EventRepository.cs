using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.DomainObjects;

namespace DertInfo.Repository
{
    public interface IEventRepository : IRepository<Event, int>
    {
        Task<ICollection<Event>> GetAllWithPrimaryImages();
        Task<ICollection<Event>> GetAvailableWithPrimaryImage();
        Task<ICollection<Event>> GetPromotedWithImages();
        Task<ICollection<Event>> GetShowcaseWithImages();
        Task<ICollection<Event>> GetAllWithPrimaryImageAndCounts();
        Task ApplyPrimaryImage(int eventId, int eventImageId);
        Task<EventImage> GetPrimaryImageForEvent(int eventId);
        Task<Event> GetEventWithImagesById(int id);
        Task<Event> GetEventWithPrimaryImageAndCountsById(int id);
        Task<IQueryable<Event>> GetEventOverviewQueryable(int eventId);
        Task<Event> GetEventOverview(int eventId);
        Task<Event> MarkDeleted(int eventId);
        Task<Event> MarkCancelled(int eventId);
        Task<Event> GetShowcaseDetailWithImages(int eventId);
        Task<ContactInfoDO> GetContactInfo(int eventId);
    }

    public class EventRepository : BaseRepository<Event, int, DertInfoContext>, IEventRepository
    {
        public EventRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }

        public async Task<IQueryable<Event>> GetEventOverviewQueryable(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Event> query = _context.Events
                .Where(e => e.Id == id)
                //.Include(e => e.Registrations).ThenInclude(r => r.TeamAttendances)
                //.Include(e => e.Registrations).ThenInclude(r => r.MemberAttendances)
                //// .Include(e => e.AttendanceClassifications)
                //.Include(e => e.Activities)
                //.Include(e => e.Competitions)
                .Include(g => g.EventImages.Where(gi => gi.IsPrimary)).ThenInclude(gi => gi.Image);

                return query;
            });

            return await task;
        }

        /// <summary>
        /// Used to get the dashboard page of the event containing counts etc.
        /// </summary>
        /// <param name="id">The Id of the event</param>
        /// <returns>Information of the event for the given Id</returns>
        /// <remarks> This function has been made obsolte as it unpacks a massive amount of data in order to get the counts. For a standard event we unpacked approx 17000 rows.</remarks>
        [Obsolete("GetEventOverview is now obsolete. Use GetEventOverviewQueryable in it's place.")]
        public async Task<Event> GetEventOverview(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Event> query = _context.Events
                .Where(e => e.Id == id)
                .Include(e => e.Registrations).ThenInclude(r => r.TeamAttendances)
                .Include(e => e.Registrations).ThenInclude(r => r.MemberAttendances)
                // .Include(e => e.AttendanceClassifications)
                .Include(e => e.Activities)
                .Include(e => e.Competitions)
                .Include(e => e.EventImages).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<ICollection<Event>> GetAllWithPrimaryImages()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Event> query = _context.Events
                .Include(e => e.EventImages.Where(i => i.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        /// <summary>
        /// Lists all events where registration is open. Private events do not have registration dates therefore should be omitted.
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<Event>> GetAvailableWithPrimaryImage()
        {
            var task = Task.Run(() =>
            {
                var now = DateTime.Now;
                var startOfToday = new DateTime(now.Year, now.Month, now.Day);
                var startOfTomorrow = startOfToday.AddDays(1);

                IQueryable<Event> query = _context.Events
                .Where(e =>
                    e.EventStartDate > startOfToday // starts today or in the future
                    && e.RegistrationOpenDate < startOfTomorrow // registration has opened
                    && e.RegistrationCloseDate > startOfToday // registration has not closed
                    && e.IsConfigured
                    && !e.IsCancelled
                )
                .Include(e => e.EventImages.Where(i => i.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }


        /// <summary>
        /// Lists all events where registration is open. Private events do not have registration dates therefore should be omitted.
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<Event>> GetPromotedWithImages()
        {
            var task = Task.Run(() =>
            {
                var now = DateTime.Now;
                var startOfToday = new DateTime(now.Year, now.Month, now.Day);
                var startOfTomorrow = startOfToday.AddDays(1);

                IQueryable<Event> query = _context.Events
                .Where(e =>
                    e.EventStartDate > startOfToday
                    && e.RegistrationOpenDate < startOfTomorrow
                    && e.RegistrationCloseDate > startOfToday
                    && e.IsConfigured
                    && e.IsPromoted
                    && !e.IsCancelled
                )
                .Include(e => e.EventImages.Where(i => i.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        /// <summary>
        /// Lists all events where registration is open. Private events do not have registration dates therefore should be omitted.
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<Event>> GetShowcaseWithImages()
        {
            var task = Task.Run(() =>
            {


            IQueryable<Event> query = _context.Events
            .Where(e => 
                e.IsConfigured
                && e.EventVisibilityType == Models.System.Enumerations.EventVisibilityType._public
                )
                .Include(e => e.EventImages.Where(i => i.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        public async Task<Event> GetShowcaseDetailWithImages(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Event> query = _context.Events
                .Where(e =>
                    e.Id == id
                    && e.IsConfigured
                    && e.EventVisibilityType == Models.System.Enumerations.EventVisibilityType._public
                    )
                    .Include(e => e.EventImages.Where(i => i.IsPrimary)).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<ICollection<Event>> GetAllWithPrimaryImageAndCounts()
        {
            var task = Task.Run(() =>
            {
                IQueryable<Event> query = _context.Events
                .Include(e => e.Registrations)
                .Include(e => e.EventImages.Where(i => i.IsPrimary)).ThenInclude(i => i.Image);

                return query.ToList();
            });

            return await task;
        }

        /// <summary>
        /// Get the event with all the images
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <remarks>
        /// This is the call that is used to get the images for the select image screen.
        /// </remarks>
        public async Task<Event> GetEventWithImagesById(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Event> query = _context.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventImages).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<Event> GetEventWithPrimaryImageAndCountsById(int id)
        {
            var task = Task.Run(() =>
            {
                IQueryable<Event> query = _context.Events
                .Where(e => e.Id == id)
                .Include(e => e.Registrations)
                .Include(e => e.EventImages.Where(i => i.IsPrimary)).ThenInclude(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<EventImage> GetPrimaryImageForEvent(int eventId)
        {
            var task = Task.Run(() =>
            {
                IQueryable<EventImage> query = _context.EventImages
                .Where(gi => gi.EventId == eventId && gi.IsPrimary)
                .Include(i => i.Image);

                return query.FirstOrDefault();
            });

            return await task;
        }

        public async Task<ContactInfoDO> GetContactInfo(int eventId)
        {
            var task = Task.Run(() =>
            {
                var query = _context.Events
                .Where(e => e.Id == eventId)
                .Select(e => new {
                    e.ContactName,
                    e.ContactEmail,
                    e.ContactTelephone
                });

                var result = query.FirstOrDefault();

                return new ContactInfoDO
                {
                    ContactName = result.ContactName,
                    ContactEmail = result.ContactEmail,
                    ContactTelephone = result.ContactTelephone,
                };
            });

            return await task;
        }


        public async Task ApplyPrimaryImage(int id, int eventImageId)
        {

            var task = Task.Run(() =>
            {
                _context.EventImages
                    .Where(gi => gi.EventId == id).ToList()
                    .ForEach(gi =>
                        gi.IsPrimary = gi.Id == eventImageId
                     );

                _context.SaveChanges();
            });

            await task;
        }

        public async Task<Event> MarkDeleted(int eventId)
        {
            var task = Task.Run(() =>
            {
                var myEvent = _context.Events.Find(eventId);

                myEvent.IsDeleted = true;

                _context.SaveChanges();

                return myEvent;
            });

            return await task;
        }

        public async Task<Event> MarkCancelled(int eventId)
        {
            var task = Task.Run(() =>
            {
                var myEvent = _context.Events.Find(eventId);

                myEvent.IsCancelled = true;

                _context.SaveChanges();

                return myEvent;
            });

            return await task;
        }


    }
}
