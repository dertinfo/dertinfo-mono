using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodSubmissions
{
    public interface IDodSubmissionReader
    {
        Task<ICollection<DodSubmission>> ListAll();
        Task<bool> HasGroupEntered(int groupId);
    }

    public class DodSubmissionReader : IDodSubmissionReader
    {
        IAuthService _authService;
        IDodSubmissionRepository _dodSubmissionRepository;
        IDertInfoUser _user;

        public DodSubmissionReader(
            IAuthService authService,
            IDodSubmissionRepository dodSubmissionRepository,
            IDertInfoUser user
            )
        {
            _authService = authService;
            _dodSubmissionRepository = dodSubmissionRepository;
            _user = user;
        }

        public async Task<bool> HasGroupEntered(int groupId)
        {
            var groups = await this._dodSubmissionRepository.Find(s => s.GroupId == groupId);

            return groups.Count > 0;
        }

        public async Task<ICollection<DodSubmission>> ListAll()
        {
            var dodSubmissions = await _dodSubmissionRepository.GetAllWithPrimaryImage();
            return dodSubmissions;
        }
    }
}
