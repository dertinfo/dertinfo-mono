using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.EventSettings
{
    public interface IEventSettingService
    {
        Task<EventSetting> GetByType(int eventId, EventSettingType eventSettingType);
    }

    public class EventSettingService : IEventSettingService
    {
        IEventSettingRepository _eventSettingRepository;

        public EventSettingService(IEventSettingRepository eventSettingRepository)
        {
            _eventSettingRepository = eventSettingRepository;
        }

        public async Task<EventSetting> GetByType(int eventId, EventSettingType eventSettingType)
        {
            var result = await _eventSettingRepository.Find(es => es.EventId == eventId && es.Ref == eventSettingType.ToString());

            if (result.Count != 1)
            {
                throw new InvalidOperationException("Single event setting instance cannot be found");
            }

            return result[0];
        }
    }
}
