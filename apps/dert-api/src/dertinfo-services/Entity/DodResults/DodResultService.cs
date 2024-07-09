using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodResults
{
    public interface IDodResultService
    {
        Task<bool> IsOfficalJudgePasswordValid(string password);
        Task<DodResult> Create(DodResult dodResult);
        Task<ICollection<int>> GetJudgedDancesByUserId(int id);
        Task<DodTeamCollatedResultPairDO> GetOfficialAndPublicResult();
        Task<DodGroupResultsDO> GetResultsForGroup(int groupId);
        Task<bool> CreateComplaint(DodResultComplaint myDodResultComplaint);
        Task<DodResult> GetResultById(int resultId);
        Task<ICollection<DodResultComplaintDO>> GetOpenComplaints();
        Task<ICollection<DodResultComplaintDO>> GetClosedComplaints();
        Task<bool> ValidateComplaint(int complaintId);
        Task<bool> RejectComplaint(int complaintId);
        Task<DodUserResultsDO> GetResultsForJudge(int judgeId);
        Task<DodGroupResultsDO> GetResultsForSubmission(int submissionId);
        Task<bool> ClearResultCache();
    }

    public class DodResultService : IDodResultService
    {
        IDodResultCache _dodResultCache;
        IDodResultCreator _dodResultCreator;
        IDodResultJudgeValidator _dodResultJudgeValidator;
        IDodResultReader _dodResultReader;
        IDodResultUpdater _dodResultUpdater;

        public DodResultService(
            IDodResultCache dodResultCache,
            IDodResultCreator dodResultCreator,
            IDodResultJudgeValidator dodResultJudgeValidator,
            IDodResultReader dodResultReader,
            IDodResultUpdater dodResultUpdater
            )
        {
            _dodResultCache = dodResultCache;
            _dodResultCreator = dodResultCreator;
            _dodResultJudgeValidator = dodResultJudgeValidator;
            _dodResultReader = dodResultReader;
            _dodResultUpdater = dodResultUpdater;
        }

        public async Task<DodResult> Create(DodResult dodResult)
        {
            return await this._dodResultCreator.Create(dodResult);
        }

        public async Task<bool> IsOfficalJudgePasswordValid(string password)
        {
            return await this._dodResultJudgeValidator.IsOfficalJudgePasswordValid(password);
        }

        public async Task<ICollection<int>> GetJudgedDancesByUserId(int userId)
        {
            return await this._dodResultReader.GetJudgedDanceIdsByUser(userId);
        }

        public async Task<DodTeamCollatedResultPairDO> GetOfficialAndPublicResult()
        {
            // Return from Cache
            var cache = await _dodResultCache.GetCache();
            if (cache != null) return cache;

            // Build, Cache and Return
            var result = await this._dodResultReader.GetOfficialAndPublicResult();
            await _dodResultCache.CreateCache(result);
            return result;

        }

        public async Task<DodGroupResultsDO> GetResultsForGroup(int groupId)
        {
            return await this._dodResultReader.GetResultsForGroup(groupId);
        }

        public async Task<DodUserResultsDO> GetResultsForJudge(int judgeId)
        {
            return await this._dodResultReader.GetResultsForJudge(judgeId);
        }

        public async Task<DodGroupResultsDO> GetResultsForSubmission(int submissionId)
        {
            return await this._dodResultReader.GetResultsForSubmission(submissionId);
        }

        public async Task<bool> CreateComplaint(DodResultComplaint myDodResultComplaint)
        {
            return await this._dodResultUpdater.AttachNewComplaint(myDodResultComplaint);
        }

        public async Task<DodResult> GetResultById(int resultId)
        {
            return await this._dodResultReader.GetResultById(resultId);
        }

        public async Task<ICollection<DodResultComplaintDO>> GetOpenComplaints()
        {
            return await this._dodResultReader.GetOpenComplaintsWithResult();
        }

        public async Task<ICollection<DodResultComplaintDO>> GetClosedComplaints()
        {
            return await this._dodResultReader.GetClosedComplaintsWithResult();
        }

        public async Task<bool> ValidateComplaint(int complaintId)
        {
            return await this._dodResultUpdater.ValidateComplaint(complaintId);
        }

        public async Task<bool> RejectComplaint(int complaintId)
        {
            return await this._dodResultUpdater.RejectComplaint(complaintId);
        }

        public async Task<bool> ClearResultCache()
        {
            return await this._dodResultCache.ClearCache();
        }
    }
}
