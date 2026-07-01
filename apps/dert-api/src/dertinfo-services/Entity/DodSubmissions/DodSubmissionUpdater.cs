using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using DertInfo.Repository;
using System;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodSubmissions
{
    public interface IDodSubmissionUpdater
    {
        Task<bool> AddNewResultToCumulativeScores(DodResultDO newResult);
        Task<DodSubmission> Update(DodSubmission myDodSubmission);
    }

    public class DodSubmissionUpdater : IDodSubmissionUpdater
    {
        IDodSubmissionRepository _dodSubmissionRepository;

        public DodSubmissionUpdater(
            IDodSubmissionRepository dodSubmissionRepository
            )
        {
            _dodSubmissionRepository = dodSubmissionRepository;
        }


        public async Task<bool> AddNewResultToCumulativeScores(DodResultDO newResult)
        {
            var submission = await this._dodSubmissionRepository.GetById(newResult.SubmissionId);

            submission.CumulativeNumberOfResults = ++submission.CumulativeNumberOfResults;

            return await this._dodSubmissionRepository.Update(submission);
        }

        public async Task<DodSubmission> Update(DodSubmission updatedSubmission)
        {
            var originalSubmission = await _dodSubmissionRepository.GetByIdWithPrimaryImage(updatedSubmission.Id);

            if (originalSubmission == null) { throw new InvalidOperationException("Talk Could Not Be Found"); }

            if (originalSubmission.GroupId != updatedSubmission.GroupId)
            {
                originalSubmission.GroupId = updatedSubmission.GroupId;
            }

            if (originalSubmission.EmbedLink != updatedSubmission.EmbedLink)
            {
                originalSubmission.EmbedLink = updatedSubmission.EmbedLink;
            }

            if (originalSubmission.EmbedOrigin != updatedSubmission.EmbedOrigin)
            {
                originalSubmission.EmbedOrigin = updatedSubmission.EmbedOrigin;
            }

            if (originalSubmission.DertYearFrom != updatedSubmission.DertYearFrom)
            {
                originalSubmission.DertYearFrom = updatedSubmission.DertYearFrom;
            }

            if (originalSubmission.DertVenueFrom != updatedSubmission.DertVenueFrom)
            {
                originalSubmission.DertVenueFrom = updatedSubmission.DertVenueFrom;
            }

            if (originalSubmission.IsPremier != updatedSubmission.IsPremier)
            {
                originalSubmission.IsPremier = updatedSubmission.IsPremier;
            }

            if (originalSubmission.IsChampionship != updatedSubmission.IsChampionship)
            {
                originalSubmission.IsChampionship = updatedSubmission.IsChampionship;
            }

            if (originalSubmission.IsOpen != updatedSubmission.IsOpen)
            {
                originalSubmission.IsOpen = updatedSubmission.IsOpen;
            }

            await _dodSubmissionRepository.Update(originalSubmission);

            return originalSubmission;
        }
    }
}
