using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DertInfo.Models.Database;
using DertInfo.Repository;

namespace DertInfo.Services.Entity.DodTalks
{
    public interface IDodTalkService
    {
        Task<ICollection<DodTalk>> ListAll();
        Task<DodTalk> Create(DodTalk dodTalk);

        Task<DodTalk> Update(DodTalk dodTalk);

        Task<DodTalk> Delete(int dodTalkId);

    }

    public class DodTalkService : IDodTalkService
    {
        IDodTalkRepository _dodTalkRepository;

        public DodTalkService(
            IDodTalkRepository _dodTalkRepository
            )
        {
            this._dodTalkRepository = _dodTalkRepository;
        }

        public async Task<ICollection<DodTalk>> ListAll()
        {
            var dodTalks = await _dodTalkRepository.GetAll();
            return dodTalks;
        }

        public async Task<DodTalk> Create(DodTalk dodTalk)
        {
            dodTalk = await _dodTalkRepository.Add(dodTalk);

            return dodTalk;
        }

        public async Task<DodTalk> Delete(int dodTalkId)
        {
            var dodTalk = await _dodTalkRepository.GetById(dodTalkId);

            // Does a real delete which is unusal for this application.
            await _dodTalkRepository.DeleteById(dodTalkId);

            return dodTalk;
        }

        

        public async Task<DodTalk> Update(DodTalk updatedTalk)
        {
            var originalTalk = await _dodTalkRepository.GetById(updatedTalk.Id);

            if (originalTalk == null) { throw new InvalidOperationException("Talk Could Not Be Found"); }

            if (originalTalk.Title != updatedTalk.Title)
            {
                originalTalk.Title = updatedTalk.Title;
            }

            if (originalTalk.SubTitle != updatedTalk.SubTitle)
            {
                originalTalk.SubTitle = updatedTalk.SubTitle;
            }

            if (originalTalk.Description != updatedTalk.Description)
            {
                originalTalk.Description = updatedTalk.Description;
            }

            if (originalTalk.BroadcastDateTime != updatedTalk.BroadcastDateTime)
            {
                originalTalk.BroadcastDateTime = updatedTalk.BroadcastDateTime;
            }

            if (originalTalk.BroadcastDateTime != updatedTalk.BroadcastDateTime)
            {
                originalTalk.BroadcastDateTime = updatedTalk.BroadcastDateTime;
            }

            if (originalTalk.BroadcastWebLink != updatedTalk.BroadcastWebLink)
            {
                originalTalk.BroadcastWebLink = updatedTalk.BroadcastWebLink;
            }

            await _dodTalkRepository.Update(originalTalk);

            return originalTalk;
        }
    }
}
