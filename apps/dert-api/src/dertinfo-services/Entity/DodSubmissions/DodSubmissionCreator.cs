using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository;

namespace DertInfo.Services.Entity.DodSubmissions
{
    public interface IDodSubmissionCreator
    {
        Task<DodSubmission> Create(DodSubmission dodSubmission);
    }

    public class DodSubmissionCreator : IDodSubmissionCreator
    {
        IAuthService _authService;
        IDodSubmissionRepository _dodSubmissionRepository;
        IDertInfoUser _user;

        public DodSubmissionCreator(
            IAuthService authService,
            IDodSubmissionRepository dodSubmissionRepository,
            IDertInfoUser user
            )
        {
            _authService = authService;
            _dodSubmissionRepository = dodSubmissionRepository;
            _user = user;
        }

        /// <summary>
        /// Create a new DertOfDerts submission with the information. 
        /// </summary>
        /// <param name="dodSubmision"></param>
        /// <returns></returns>
        public async Task<DodSubmission> Create(DodSubmission dodSubmision)
        {
            var dodSubmission = await _dodSubmissionRepository.Add(dodSubmision);

            // We need to get the group detail with this else the group name will not be there.
            dodSubmission = await this._dodSubmissionRepository.GetByIdWithPrimaryImage(dodSubmission.Id);

            return dodSubmission;
        }
    }
}
