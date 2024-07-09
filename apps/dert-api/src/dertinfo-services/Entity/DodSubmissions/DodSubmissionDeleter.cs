using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodSubmissions
{
    public interface IDodSubmissionDeleter
    {
        Task<DodSubmission> Delete(int dodSubmissionId);
    }

    public class DodSubmissionDeleter : IDodSubmissionDeleter
    {
        IAuthService _authService;
        IDodSubmissionRepository _dodSubmissionRepository;
        IDertInfoUser _user;

        public DodSubmissionDeleter(
            IAuthService authService,
            IDodSubmissionRepository dodSubmissionRepository,
            IDertInfoUser user
            )
        {
            _authService = authService;
            _dodSubmissionRepository = dodSubmissionRepository;
            _user = user;
        }

        public async Task<DodSubmission> Delete(int dodSubmissionId)
        {
            var dodSubmission = await _dodSubmissionRepository.GetById(dodSubmissionId);

            // Does a real delete which is unusal for this application.
            await _dodSubmissionRepository.DeleteById(dodSubmissionId);
                
            return dodSubmission;
        }
    }

}
