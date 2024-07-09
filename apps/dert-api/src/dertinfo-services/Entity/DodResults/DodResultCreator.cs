using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using DertInfo.Repository;
using DertInfo.Services.Entity.DodSubmissions;
using DertInfo.Services.Entity.SystemSettings;
using System;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodResults
{
    public interface IDodResultCreator
    {
        Task<DodResult> Create(DodResult dodResult);
    }

    public class DodResultCreator : IDodResultCreator
    {

        IDodResultRepository _dodResultRepository;
        IDodSubmissionService _dodSubmissionService;
        ISystemSettingService _systemSettingService;

        public DodResultCreator(
            IDodResultRepository dodResultRepository,
            IDodSubmissionService dodSubmissionService,
            ISystemSettingService systemSettingService
            )
        {
            this._dodResultRepository = dodResultRepository;
            this._dodSubmissionService = dodSubmissionService;
            this._systemSettingService = systemSettingService;
        }

        public async Task<DodResult> Create(DodResult dodResult)
        {
            var resultsPublishedSetting = await this._systemSettingService.GetByKey("SYSTEM-DOD-RESULTSPUBLISHED");
            if (bool.Parse(resultsPublishedSetting.Value)) { throw new InvalidOperationException("New results cannot be created once the results have been published"); }

            var result = await this._dodResultRepository.Add(dodResult);

            // Tell the submission service that another result has been recieved. 
            DodResultDO newResult = new DodResultDO()
            {
                SubmissionId = result.SubmissionId,
                MusicScore = result.MusicScore,
                SteppingScore = result.SteppingScore,
                SwordHandlingScore = result.SwordHandlingScore,
                DanceTechniqueScore = result.DanceTechniqueScore,
                BuzzScore = result.BuzzScore,
                CharactersScore = result.CharactersScore,
            };

            var succeeded = await this._dodSubmissionService.InformNewResult(newResult);
            if (!succeeded) { throw new Exception("Submission results have not been added to totals"); }

            return result;
        }
    }
}
