using DertInfo.CrossCutting.Auth;
using DertInfo.CrossCutting.Configuration;
using DertInfo.CrossCutting.Connection;
using DertInfo.Models.Database;
using DertInfo.Models.System;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using DertInfo.Services.Entity.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllForShowcase();
        Task<IEnumerable<Team>> GetConfirmedByEvent(int eventId);
        Task<Team> GetDetail(int teamId);
        Task<Team> UpdateTeam(Team myTeam);
        Task<TeamImage> AttachImage(TeamImage myTeamImage);
        Task ObfuscateTeamById(int id);
        Task<Team> GetForAuthorization(int teamId);
        Task<IEnumerable<Team>> GetByGroupWithImages(int id);
    }

    public class TeamService : ITeamService
    {
        IDertInfoConfiguration _dertInfoConfiguration;
        IGroupRepository _groupRepository;
        IImageRepository _imageRepository;
        IImageService _imageService;
        ITeamRepository _teamRepository;


        public TeamService(
            IDertInfoConfiguration dertInfoConfiguration,
            IGroupRepository groupRepository,
            IImageRepository imageRepository,
            IImageService imageService,
            ITeamRepository teamRepository
            )
        {
            _dertInfoConfiguration = dertInfoConfiguration;
            _groupRepository = groupRepository;
            _imageRepository = imageRepository;
            _imageService = imageService;
            _teamRepository = teamRepository;
        }

        public async Task<Team> GetForAuthorization(int teamId)
        {
            return await this._teamRepository.GetById(teamId);
        }

        /// <summary>
        /// Should return all the events that the user should have access to in the context of root access.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Team>> GetAllForShowcase()
        {
            var allTeams = await _teamRepository.GetAllWithImagesAndRegistrations();

            foreach (var team in allTeams)
            {
                team.TeamAttendances = team.TeamAttendances.Where(ta => ta.Registration.FlowState == RegistrationFlowState.Confirmed || ta.Registration.FlowState == RegistrationFlowState.Closed).ToList();
            }

            return allTeams.Where(t => t.ShowShowcase);
        }

        public async Task<IEnumerable<Team>> GetByGroupWithImages(int groupId)
        {
            return await this._teamRepository.GetByGroupWithImages(groupId);
        }

        public async Task<Team> GetDetail(int teamId)
        {
            var team = await _teamRepository.GetTeamDetail(teamId);

            if (team.TeamImages.Count == 0)
            {
                var groupPrimaryImage = await this._groupRepository.GetPrimaryImageForGroup(team.GroupId);
                var groupFallbackImage = new TeamImage
                {
                    Id = 0,
                    TeamId = teamId,
                    IsPrimary = true,
                    Image = groupPrimaryImage.Image,
                    ImageId = groupPrimaryImage.Id
                };

                team.TeamImages.Add(groupFallbackImage);
            }

            return team;

        }

        public async Task<Team> UpdateTeam(Team updatedTeam)
        {
            var originalTeam = await _teamRepository.GetById(updatedTeam.Id);

            if (originalTeam == null) { throw new InvalidOperationException("Team Could Not Be Found"); }
            if (originalTeam.GroupId != updatedTeam.GroupId) { throw new InvalidOperationException("It is not permitted to update a team across group"); }

            if (originalTeam.TeamName != updatedTeam.TeamName)
            {
                originalTeam.TeamName = updatedTeam.TeamName;
            }

            if (originalTeam.TeamBio != updatedTeam.TeamBio)
            {
                originalTeam.TeamBio = updatedTeam.TeamBio;
            }

            if (originalTeam.IsDeleted != updatedTeam.IsDeleted)
            {
                // We do not permit delete in an update call this has to be implemented explictly
            }

            await _teamRepository.Update(originalTeam);

            return originalTeam;
        }

        public async Task<TeamImage> AttachImage(TeamImage myTeamImage)
        {
            var team = await _teamRepository.GetTeamWithImagesAndCountsById(myTeamImage.TeamId);

            // If we are attaching an image to a team then we should always remove the default one if it is present.
            var defaultImage = await _imageService.GetDefaultGroupImage();
            var filteredImages = team.TeamImages.Where(ti => ti.ImageId != defaultImage.Id).ToList();
            team.TeamImages = filteredImages; // detaches the default

            // Ensure that we are not attaching the same image multiple times
            bool permitAddition = true;
            if (filteredImages.Count > 0)
            {
                foreach (var teamImage in team.TeamImages)
                {
                    permitAddition = teamImage.ImageId != myTeamImage.ImageId ? permitAddition : false; // Check not already attached
                    teamImage.IsPrimary = teamImage.ImageId == myTeamImage.ImageId; // set the primary as we iterate
                }
            }

            // If the attachement is not a duplicate then add it.
            if (permitAddition)
            {
                var imageFound = await _imageRepository.GetById(myTeamImage.ImageId);
                myTeamImage.Image = imageFound ?? throw new ArgumentException("Image specified Cannot be found");
                team.TeamImages.Add(myTeamImage);
            }

            await _teamRepository.Update(team); // Update with new primary regardless
            return team.TeamImages.Where(ti => ti.ImageId == myTeamImage.ImageId).First();
        }

        public async Task<IEnumerable<Team>> GetAttendancesByEventIdAndGroupId(int eventId, int groupId)
        {
            return await _teamRepository.GetByGroupAttendedEvent(eventId, groupId);
        }

        public async Task ObfuscateTeamById(int teamId)
        {
            var team = await _teamRepository.GetById(teamId);

            // Team Images
            foreach (var ti in team.TeamImages)
            {
                // Mark each image for deletion
                ti.IsPrimary = false;
                await _imageService.MarkForDeletion(ti.ImageId);
            }

            var defaultImage = await this._imageService.GetDefaultGroupImage();
            var defaultTeamImage = new TeamImage()
            {
                ImageId = defaultImage.Id,
                TeamId = team.Id,
                IsPrimary = true
            };

            team.TeamImages.Clear();
            team.TeamImages.Add(defaultTeamImage);

            await _teamRepository.Update(team);
        }

        public Task<IEnumerable<Team>> GetConfirmedByEvent(int eventId)
        {
            return this._teamRepository.GetByEventWithImages(eventId, new List<RegistrationFlowState>() { RegistrationFlowState.Confirmed, RegistrationFlowState.Closed });
        }

        
    }
}
