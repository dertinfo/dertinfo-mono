using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodSubmissions
{
    public interface IDodSubmissionService
    {
        Task<ICollection<DodSubmission>> ListAll();
        Task<DodSubmission> Create(DodSubmission dodSubmission);
        Task<DodSubmission> DeleteSubmission(int dodSubmissionId);
        Task<bool> InformNewResult(DodResultDO newResult);
        Task<bool> HasGroupEntered(int groupId);
        Task<DodSubmission> Update(DodSubmission myDodSubmission);
    }

    public class DodSubmissionService : IDodSubmissionService
    {
        IDodSubmissionCreator _dodSubmissionCreator;
        IDodSubmissionDeleter _dodSubmissionDeleter;
        IDodSubmissionReader _dodSubmissionReader;
        IDodSubmissionUpdater _dodSubmissionUpdater;

        public DodSubmissionService(
            IDodSubmissionCreator dodSubmissionCreator,
            IDodSubmissionDeleter dodSubmissionDeleter,
            IDodSubmissionReader dodSubmissionReader,
            IDodSubmissionUpdater dodSubmissionUpdater
            )
        {
            this._dodSubmissionCreator = dodSubmissionCreator;
            this._dodSubmissionDeleter = dodSubmissionDeleter;
            this._dodSubmissionReader = dodSubmissionReader;
            this._dodSubmissionUpdater = dodSubmissionUpdater;
        }

        public async Task<ICollection<DodSubmission>> ListAll()
        {
            return await this._dodSubmissionReader.ListAll();
        }

        public async Task<DodSubmission> Create(DodSubmission dodSubmission)
        {
            return await this._dodSubmissionCreator.Create(dodSubmission);
        }

        public async Task<DodSubmission> DeleteSubmission(int dodSubmissionId)
        {
            return await this._dodSubmissionDeleter.Delete(dodSubmissionId);
        }

        public async Task<bool> InformNewResult(DodResultDO newResult)
        {
            return await this._dodSubmissionUpdater.AddNewResultToCumulativeScores(newResult);
        }

        public async Task<bool> HasGroupEntered(int groupId)
        {
            return await this._dodSubmissionReader.HasGroupEntered(groupId);
        }

        public async Task<DodSubmission> Update(DodSubmission myDodSubmission)
        {
            return await this._dodSubmissionUpdater.Update(myDodSubmission);
        }
    }
}
