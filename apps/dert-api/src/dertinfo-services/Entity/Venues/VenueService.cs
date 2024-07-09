using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IVenueService
    {

        Task<ICollection<Venue>> ListByUser();
        Task<ICollection<Venue>> ListByUser(int eventId);
    }

    public class VenueService : IVenueService
    {
        private int _currentDertEventId = 21;

        IDertInfoUser _user;
        IVenueRepository _venueRepository;

        public VenueService(IVenueRepository venueRepository, IDertInfoUser user)
        {
            _user = user;
            _venueRepository = venueRepository;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <returns></returns>
        [Obsolete("ListByUser() is obsolete please use ListByUser(eventId) instead. eventId formally hardcoded")]
        public async Task<ICollection<Venue>> ListByUser()
        {
            if (_user.AuthId != string.Empty)
            {
                /***********************************************************************/
                /* START - CODE HACKED TO SUPPORT 2017 AND MASTER - AUTH 0 ACCOUNTS    */
                /***********************************************************************/

                var masterAuth0AuthId = "auth0|58bc7cb0a0ba8d24312f92cd";
                var currentDertAuthId = "auth0|58bc7c3012c98e438135a450";

                if ((_user.ClaimsGroupAdmin != null && _user.ClaimsGroupAdmin.Count() > 0) || (_user.ClaimsGroupMember != null && _user.ClaimsGroupMember.Count() > 0))
                {

                    var groupIds = _user.ClaimsGroupAdmin.Union(_user.ClaimsGroupMember).Distinct().Select(x => int.Parse(x)).ToList();
                    return await ListVenuesByGroup(groupIds);
                }

                if (_user.AuthId == masterAuth0AuthId || _user.AuthId == currentDertAuthId)
                {
                    return await _venueRepository.Find(v => v.Event.Id == _currentDertEventId);
                }

                /***********************************************************************/
                /* START - CODE HACKED TO SUPPORT 2017 AND MASTER - AUTH 0 ACCOUNTS    */
                /***********************************************************************/

                return await _venueRepository.Find(v => v.Auth0Username == _user.AuthId);
            }
            else
            {
                throw new InvalidOperationException("VenueService - ListByUser - User must be specified.");
            }
        }

        public async Task<ICollection<Venue>> ListByUser(int eventId)
        {
            if (_user.AuthId != string.Empty)
            {

                //Super Admin Gets All Venues
                if (_user.IsSuperAdmin)
                {
                    return await _venueRepository.Find(v => v.Event.Id == eventId);
                }

                // Event Admin Gets all venues for event
                if ((_user.ClaimsEventAdmin != null && _user.ClaimsEventAdmin.Count() > 0) && _user.ClaimsEventAdmin.Any(claim => claim == eventId.ToString()))
                {
                    return await _venueRepository.Find(v => v.Event.Id == eventId);
                }

                // Venue Admin gets venues that they are admin for in the scope of the event.
                if ((_user.ClaimsVenueAdmin != null && _user.ClaimsVenueAdmin.Count() > 0))
                {
                    return await _venueRepository.Find(v => _user.ClaimsVenueAdmin.Contains(v.Id.ToString()) && v.EventId == eventId);
                }

                // Group Member Or Group Admin gets venues appropriate to group
                if ((_user.ClaimsGroupAdmin != null && _user.ClaimsGroupAdmin.Count() > 0) || (_user.ClaimsGroupMember != null && _user.ClaimsGroupMember.Count() > 0))
                {

                    var groupIds = _user.ClaimsGroupAdmin.Union(_user.ClaimsGroupMember).Distinct().Select(x => int.Parse(x)).ToList();
                    return await ListVenuesByGroup(eventId, groupIds);
                }

                // Else fallback to the user specified on the venue
                return await _venueRepository.Find(v => v.Auth0Username == _user.AuthId);
            }
            else
            {
                throw new InvalidOperationException("VenueService - ListByUser - User must be specified.");
            }
        }

        [Obsolete("ListVenuesByGroup(groupId) is obsolete please use ListVenuesByGroup(eventId,groupId) instead. eventId formally hardcoded")]
        private async Task<ICollection<Venue>> ListVenuesByGroup(IEnumerable<int> groupIds)
        {
            // obsolete - Identify the activities for the group and list the venues assosiated with the activity
            //          - listing all at this point as doesn't matter so long as the dances are only returned appropriate for the group via the team entry information.

            return await _venueRepository.Find(v => v.Event.Id == _currentDertEventId);
        }

        private async Task<ICollection<Venue>> ListVenuesByGroup(int eventId, IEnumerable<int> groupIds)
        {
            // todo - Identify the activities for the group and list the venues assosiated with the activity
            //      - listing all at this point as doesn't matter so long as the dances are only returned appropriate for the group via the team entry information.

            return await _venueRepository.Find(v => v.Event.Id == eventId);
        }
    }
}
