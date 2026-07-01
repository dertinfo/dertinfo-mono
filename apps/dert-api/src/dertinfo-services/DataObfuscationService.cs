using DertInfo.CrossCutting.Configuration;
using DertInfo.Models.Database;
using DertInfo.Services.Entity.Events;
using DertInfo.Services.Entity.Groups;
using DertInfo.Services.Entity.Images;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services
{
    public interface IDataObfuscationService
    {
        Task ObfuscateGroupInformation(int groupId);
    }

    public class DataObfuscationService : IDataObfuscationService
    {
        private IDertInfoConfiguration _dertInfoConfiguration;
        private IEventService _eventService;
        private IGroupService _groupService;
        private IImageService _imageService;
        private ITeamService _teamService;

        public DataObfuscationService(
            IDertInfoConfiguration dertInfoConfiguration,
            IEventService eventService,
            IGroupService groupService,
            IImageService imageService,
            ITeamService teamService
        )
        {
            _dertInfoConfiguration = dertInfoConfiguration;
            _eventService = eventService;
            _groupService = groupService;
            _imageService = imageService;
            _teamService = teamService;
        }

        /// <summary>
        /// Obfuscates the folowing information within a group
        /// - Obfuscates - Administrator Name
        /// - Removes - Administrator Telephone, Email
        /// - Defaults - Group Images
        /// - Defaults - Team Images
        /// - Marks - Image references as for removal to be removed from blob storage.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task ObfuscateGroupInformation(int groupId)
        {
            // Group Overview
            await _groupService.ObfuscateGroupDataById(groupId);
        }

        private string CreateStandardLengthNumeric(int baseNumber)
        {
            int standardLength = 6;
            string strBaseNumber = baseNumber.ToString();

            int difference = standardLength - strBaseNumber.Length;

            for (int i = 0; i < difference; i++)
            {
                strBaseNumber = "0" + strBaseNumber;
            }

            return strBaseNumber;
        }
    }
}
