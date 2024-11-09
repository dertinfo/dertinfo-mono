using DertInfo.CrossCutting.Auth;
using DertInfo.CrossCutting.Connection;
using DertInfo.Models.Database;
using DertInfo.Repository;
using DertInfo.Services.Entity.EventSettings;
using DertInfo.Services.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.Dances
{
    public interface IDanceService
    {
        
        Task<Dance> FindById(int danceId);
        Task<IEnumerable<Dance>> ListByVenueId(int venueId);
        Task<Dance> UpdateDanceAndScores(int danceId, ICollection<DanceScore> danceScores, bool overrun);
        Task<MarkingSheetImage> AttachMarkingSheet(int danceId, byte[] imageByteArray);
        Task<ICollection<MarkingSheetImage>> ListMarkingSheets(int danceId);
        Task<MarkingSheetImage> GetMarkingSheet(int markingSheetId);
        Task<MarkingSheetImage> DetachMarkingSheet(int markingSheetId);
        Task<ICollection<Dance>> ListByTeamAndCompetition(int teamId, int competitionId);
        Task<IEnumerable<Dance>> ListForCompetition(int competitionId);
        Task UpdateCheckedState(int id, bool scoresChecked);
        Task<int> IdentifyCompetitionForDance(int danceId);
    }

    public class DanceService : IDanceService
    {
        IDertInfoUser _user;
        IBlobStorageRepository _blobStorageRepository;
        IDanceRepository _danceRepository;
        IEventSettingService _eventSettingsService;
        ITeamRepository _teamRepository;
        IImageRepository _imageRepository;
        IMarkingSheetRepository _markingSheetRepository;
        IStorageAccountConnection _storageAccountConnection;
        IVenueRepository _venueRepository;

        public DanceService(
            IDanceRepository danceRepository,
            IEventSettingService eventSettingsService,
            ITeamRepository teamRepository,
            IMarkingSheetRepository markingSheetRepository,
            IImageRepository imageRepository,
            IBlobStorageRepository blobStorageRepository,
            IDertInfoUser user,
            IStorageAccountConnection storageAccountConnection,
            IVenueRepository venueRepository
            )
        {
            _user = user;
            _danceRepository = danceRepository;
            _eventSettingsService = eventSettingsService;
            _teamRepository = teamRepository;
            _imageRepository = imageRepository;
            _markingSheetRepository = markingSheetRepository;
            _blobStorageRepository = blobStorageRepository;
            _storageAccountConnection = storageAccountConnection;
            _venueRepository = venueRepository;
    }

        public async Task<Dance> FindById(int danceId)
        {
            if (_user.AuthId == string.Empty) { throw new InvalidOperationException("Dance Service - FindById - No User"); }

            if (_user.ClaimsVenueAdmin != null && _user.ClaimsVenueAdmin.Count() > 0)
            {
                var dance = await _danceRepository.GetDanceExpandedById(danceId);

                if (_user.ClaimsVenueAdmin.Contains(dance.VenueId.ToString()))
                {
                    return dance;
                }
            }

            if (_user.ClaimsEventAdmin != null && _user.ClaimsEventAdmin.Count() > 0)
            {
                var dance = await _danceRepository.GetDanceExpandedById(danceId);

                if (_user.ClaimsEventAdmin.Contains(dance.Venue.EventId.ToString()))
                {
                    return dance;
                }
            }

            if ((_user.ClaimsGroupAdmin != null && _user.ClaimsGroupAdmin.Count() > 0) || (_user.ClaimsGroupMember != null && _user.ClaimsGroupMember.Count() > 0))
            {
                var permittedGroupIds = _user.ClaimsGroupAdmin.Union(_user.ClaimsGroupMember).Distinct().Select(x => int.Parse(x)).ToList();

                var dance = await _danceRepository.GetDanceExpandedById(danceId);

                if (permittedGroupIds.Contains(dance.TeamAttendance.Team.GroupId))
                {
                    await dance.HideResultsIfNotPublished();
                    return dance;
                }

            }

            throw new InvalidOperationException("DanceService - FindById - User does not have access to this dance.");

        }

        public async Task<Dance> UpdateDanceAndScores(int danceId, ICollection<DanceScore> danceScores, bool overrun)
        {
            if (_user.AuthId == string.Empty) { throw new InvalidOperationException("Dance Service - UpdateDanceAndScores - No User"); }

            var dance = await _danceRepository.GetDanceExpandedById(danceId);

            if (dance == null)
            {
                throw new Exception("Dance not found");
            }

            if (!_user.ClaimsVenueAdmin.Contains(dance.VenueId.ToString()) && !_user.ClaimsEventAdmin.Contains(dance.Venue.EventId.ToString()))
            {
                throw new Exception("User is not venue admin or event admin");
            }

            if (danceScores.Count != dance.DanceScores.Count)
            {
                throw new Exception("Dance scores passed invalid");
            }

            if (dance.HasScoresChecked)
            {
                throw new Exception("Submission no longer permitted");
            }

            foreach (var danceScore in danceScores)
            {
                var dbDanceScore = dance.DanceScores.Where(ds => ds.ScoreCategoryId == danceScore.ScoreCategoryId).First();
                dbDanceScore.MarkGiven = danceScore.MarkGiven;
                dbDanceScore.DateModified = DateTime.Now;
                dbDanceScore.ModifiedBy = _user.AuthId;
            }

            dance.Overrun = overrun;
            dance.HasScoresEntered = true;
            dance.HasScoresChecked = false;
            dance.DateScoresEntered = DateTime.Now;
            dance.ScoresEnteredBy = _user.AuthId;
            

            await _danceRepository.Update(dance);

            return dance;
        }

        public async Task<IEnumerable<Dance>> ListByVenueId(int venueId)
        {
            Venue venue = await _venueRepository.GetById(venueId);

            // If they are super admin then they get all dances
            if (_user.IsSuperAdmin) {
                return await _danceRepository.GetDancesExpandedByVenueId(venueId);
            }

            // If they are venue admin then they get all dances
            if (_user.ClaimsVenueAdmin != null && _user.ClaimsVenueAdmin.Count() > 0 && _user.ClaimsVenueAdmin.Contains(venueId.ToString()))
            {
                return await _danceRepository.GetDancesExpandedByVenueId(venueId);
            }

            if (_user.ClaimsEventAdmin != null && _user.ClaimsEventAdmin.Count() > 0 && _user.ClaimsEventAdmin.Contains(venue.EventId.ToString()))
            {
                return await _danceRepository.GetDancesExpandedByVenueId(venueId);
            }

            //If they are a group admin or group member they get all dances for the group
            if ((_user.ClaimsGroupAdmin != null && _user.ClaimsGroupAdmin.Count() > 0) || (_user.ClaimsGroupMember != null && _user.ClaimsGroupMember.Count() > 0))
            {
                var groupIds = _user.ClaimsGroupAdmin.Union(_user.ClaimsGroupMember).Distinct().Select(x => int.Parse(x)).ToList();

                // todo - identify the dances that are for all the team entries under the group for the event
                List<int> teamIds = new List<int>();

                foreach (var g in groupIds)
                {
                    var teams = await _teamRepository.GetByGroupAttendedEvent(venue.EventId, g);
                    var groupTeamIds = teams.Select(t => t.Id).ToList();
                    teamIds = teamIds.Union(groupTeamIds).ToList();
                }

                IEnumerable<Dance> dances = new List<Dance>();
                foreach (var teamId in teamIds)
                {
                    var teamDances = await this.ListByTeamAndVenue(teamId, venueId);
                    dances = dances.Union(teamDances);
                }

                return dances;
            }

            throw new InvalidOperationException("Dance Service - ListByVenueId - No Matching Cases Met");
        }

        public async Task<ICollection<Dance>> ListByTeamAndCompetition(int teamId, int competitionId)
        {
            var dances = await _danceRepository.GetDancesExpandedByTeamIdAndCompetitionId(teamId, competitionId);
            return dances;
        }

        public async Task<ICollection<Dance>> ListByTeamAndVenue(int teamId, int venueId)
        {
            var dances = await _danceRepository.GetDancesExpandedByTeamIdAndVenueId(teamId, venueId);
            return dances;
        }

        // todo - this needs some user protection to ensure that only authorised users can attach images (endpoint authenticated).
        public async Task<MarkingSheetImage> AttachMarkingSheet(int danceId, byte[] imageByteArray)
        {
            var dance = await _danceRepository.GetDanceExpandedById(danceId);

            var connection = _storageAccountConnection.getImagesStorageConnectionString();
            var blobContainer = _storageAccountConnection.getScoreSheetsContainer();
            var blobPath = _storageAccountConnection.getOriginalsFolder();
            var blobName = _storageAccountConnection.createScoreSheetFileName(danceId);

            // Upload image to Azure
            var resourceUri = await _blobStorageRepository.UploadFileToBlob(imageByteArray, connection, blobContainer, blobPath, blobName);

            var image = new Image()
            {
                Container = blobContainer,
                BlobPath = blobPath,
                BlobName = blobName,
                Extension = "jpg"
            };
            image = await _imageRepository.Add(image);

            var markingSheetImage = new MarkingSheetImage();
            markingSheetImage.DanceId = danceId;
            markingSheetImage.Image = image;
            markingSheetImage.AccessToken = dance.AccessToken; // apply access token

            //Save To Database - MarkingSheetImage & Image
            await _markingSheetRepository.Add(markingSheetImage);

            return markingSheetImage;
        }

        public async Task<ICollection<MarkingSheetImage>> ListMarkingSheets(int danceId)
        {
            return await _markingSheetRepository.GetMarkingSheetsExpandedByDanceId(danceId);
        }

        public async Task<MarkingSheetImage> GetMarkingSheet(int markingSheetId)
        {
            return await _markingSheetRepository.GetById(markingSheetId);
        }

        /// <summary>
        /// Delete the marking sheet from the database and the assosiated file in the storage account.
        /// </summary>
        /// <param name="markingSheetId"></param>
        /// <returns></returns>
        // todo - this needs some user protection to ensure that only authorised users can detach images (endpoint authenticated).
        public async Task<MarkingSheetImage> DetachMarkingSheet(int markingSheetId)
        {
            var markingSheet = await _markingSheetRepository.GetById(markingSheetId);

            if (markingSheet != null)
            {
                var connection = _storageAccountConnection.getImagesStorageConnectionString();
                var blobContainer = _storageAccountConnection.getScoreSheetsContainer();
                var blobPath = markingSheet.Image.BlobPath;
                var blobName = markingSheet.Image.BlobName;


                // Note: that this will only remove the original and not all the sizes.
                await _blobStorageRepository.RemoveFileFromBlob(connection, blobContainer, blobPath, blobName);

                var completed = await _markingSheetRepository.Delete(markingSheet);

                if (completed)
                {
                    return markingSheet;
                }
            }
            else
            {
                throw new NullReferenceException("Marking Sheet " + markingSheetId + "Not Found");
            }

            throw new Exception("DetachMarkingSheet - Failed");
        }

        public async Task<IEnumerable<Dance>> ListForCompetition(int competitionId)
        {
            var dances = await _danceRepository.GetDancesExpandedByCompetitionId(competitionId);

            foreach (var dance in dances)
            {
                dance.OrderScoresByScoreCategory();
            }
            // note - we use this here to ensure that when the client renders the results then the categories appear in the right order
            //      - this is so that it matches the order of the collated results. We could do this on the client and
            //      - probably should however for now due to ease it can be done here more quickly. Cemmenting for dept tracking

            return dances;
        }

        public async Task UpdateCheckedState(int danceId, bool hasScoredChecked)
        {
            var dance = await _danceRepository.GetForAuthorization(danceId);

            if (dance == null)
            {
                throw new Exception("Dance not found");
            }

            if (!_user.ClaimsEventAdmin.Contains(dance.Venue.EventId.ToString()))
            {
                throw new Exception("User is not event admin");
            }

            dance.HasScoresChecked = hasScoredChecked;

            await _danceRepository.Update(dance);
        }

        public async Task<int> IdentifyCompetitionForDance(int danceId)
        {
            var dance = await _danceRepository.GetForAuthorization(danceId);

            return dance.CompetitionId;
        }
    }
}
