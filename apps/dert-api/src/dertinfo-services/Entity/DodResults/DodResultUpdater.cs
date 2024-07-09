using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodResults
{

    public interface IDodResultUpdater
    {
        Task<bool> AttachNewComplaint(DodResultComplaint dodResultComplaint);
        Task<bool> ValidateComplaint(int complaintId);
        Task<bool> RejectComplaint(int complaintId);
    }

    public class DodResultUpdater : IDodResultUpdater
    {
        IDodResultRepository _dodResultRepository;
        IDodResultComplaintRepository _dodResultComplaintRepository;

        public DodResultUpdater(
            IDodResultRepository dodResultRepository,
            IDodResultComplaintRepository dodResultComplaintRepository
            )
        {
            _dodResultRepository = dodResultRepository;
            _dodResultComplaintRepository = dodResultComplaintRepository;
        }

        public async Task<bool> AttachNewComplaint(DodResultComplaint dodResultComplaint)
        {
            var resultComplaint = await this._dodResultComplaintRepository.Add(dodResultComplaint);

            // Update the flag on the result. So that we can use this flag to omit when being requested by group members.
            var result = await this._dodResultRepository.GetById(dodResultComplaint.DodResultId);
            result.HasOutstandingComplaint = true;
            return await this._dodResultRepository.Update(result);
        }

        public async Task<bool> ValidateComplaint(int complaintId)
        {
            var originalComplaint = await _dodResultComplaintRepository.GetById(complaintId);

            if (originalComplaint == null) { throw new InvalidOperationException("Complaint Could Not Be Found"); }

            if (originalComplaint.IsResolved != true)
            {
                originalComplaint.IsResolved = true;
                originalComplaint.IsValidated = true;
                var resultActionTaken = await this.UpdateResultForComplaintValidation(originalComplaint.DodResultId);
                var complaintHandled = await _dodResultComplaintRepository.Update(originalComplaint);

                return true;
            }

            return false;
        }

        public async Task<bool> RejectComplaint(int complaintId)
        {
            var originalComplaint = await _dodResultComplaintRepository.GetById(complaintId);

            if (originalComplaint == null) { throw new InvalidOperationException("Complaint Could Not Be Found"); }

            if (originalComplaint.IsResolved != true)
            {
                originalComplaint.IsResolved = true;
                originalComplaint.IsRejected = true;
                var resultActionTaken = await this.UpdateResultForComplaintRejection(originalComplaint.DodResultId);
                var complaintHandled = await _dodResultComplaintRepository.Update(originalComplaint);

                return true;
            }

            return false;

        }

        /// <summary>
        /// Clear the has complaint state on the result if the only complaint and discontinue to include the score in the results. 
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        private async Task<bool> UpdateResultForComplaintValidation(int resultId)
        {
            var allComplaints = await _dodResultComplaintRepository.Find(c => c.DodResultId == resultId && !c.IsResolved);

            if (allComplaints.Count == 1) 
            {
                // There is only 1 unresolved complaint related to this result.
                var onlyComplaint = allComplaints.First();

                var result = await _dodResultRepository.GetById(onlyComplaint.DodResultId);

                if (result.IsOfficial) { throw new Exception("Cannot remove a single result where the result is official. You must either reject all results from the official judge or none."); }
  
                result.HasOutstandingComplaint = false;
                result.IncludeInScores = false;

                return await this._dodResultRepository.Update(result);
            }

            return false;
        }

        /// <summary>
        /// Clear the has complaint state on the result if the only complaint clear the outstanding complaint on the result.
        /// </summary>
        /// <param name="resultId"></param>
        /// <returns></returns>
        private async Task<bool> UpdateResultForComplaintRejection(int resultId) 
        {
            var allComplaints = await _dodResultComplaintRepository.Find(c => c.DodResultId == resultId && !c.IsResolved);

            if (allComplaints.Count == 1)
            {
                // There is only 1 unresolved complaint related to this result.
                var onlyComplaint = allComplaints.First();

                var result = await _dodResultRepository.GetById(onlyComplaint.DodResultId);

                result.HasOutstandingComplaint = false;
                // result.IncludeInScores = true; // Will already be true.

                return await this._dodResultRepository.Update(result);
            }

            return false;
        }

        
    }
}
