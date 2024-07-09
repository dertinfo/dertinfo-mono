using DertInfo.Models.Database;
using DertInfo.Models.DomainObjects.DertOfDerts;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.DodResults
{

    public interface IDodResultReader
    {
        Task<ICollection<int>> GetJudgedDanceIdsByUser(int userId);
        Task<DodTeamCollatedResultPairDO> GetOfficialAndPublicResult();
        Task<DodGroupResultsDO> GetResultsForGroup(int groupId);
        Task<DodResult> GetResultById(int resultId);
        Task<ICollection<DodResultComplaintDO>> GetOpenComplaintsWithResult(); // note - Should be in own reader
        Task<ICollection<DodResultComplaintDO>> GetClosedComplaintsWithResult(); // note - Should be in own reader
        Task<DodGroupResultsDO> GetResultsForSubmission(int submissionId);
        Task<DodUserResultsDO> GetResultsForJudge(int judgeId);
    }

    public class DodResultReader: IDodResultReader
    {
        IDodResultCollator _dodResultCollator;
        IDodResultRepository _dodResultRepository;
        IDodSubmissionRepository _dodSubmissionRepository;
        IDodResultComplaintRepository _dodResultComplaintRepository;
        IDodUserRepository _dodUserRepository;

        public DodResultReader(
            IDodResultCollator dodResultCollator,
            IDodResultRepository dodResultRepository,
            IDodSubmissionRepository dodSubmissionRepository,
            IDodResultComplaintRepository dodResultComplaintRepository,
            IDodUserRepository dodUserRepository
        )
        {
            _dodResultCollator = dodResultCollator;
            _dodResultRepository = dodResultRepository;
            _dodSubmissionRepository = dodSubmissionRepository;
            _dodResultComplaintRepository = dodResultComplaintRepository;
            _dodUserRepository = dodUserRepository;
        }

        public async Task<ICollection<DodResultComplaintDO>> GetOpenComplaintsWithResult()
        {
            var dodResultComplaints = await this._dodResultComplaintRepository.GetUnresolvedWithResult();

            var dodResultComplaintDos = new List<DodResultComplaintDO>();
            foreach (var complaint in dodResultComplaints)
            {

                var dodResultComplaintDo = new DodResultComplaintDO();
                dodResultComplaintDo.DodResultId = complaint.DodResultId;
                dodResultComplaintDo.DodSubmissionId = complaint.DodResult.SubmissionId;
                dodResultComplaintDo.Id = complaint.Id;
                dodResultComplaintDo.ForScores = complaint.ForScores;
                dodResultComplaintDo.ForComments = complaint.ForComments;
                dodResultComplaintDo.IsResolved = complaint.IsResolved;
                dodResultComplaintDo.Notes = complaint.Notes;
                dodResultComplaintDo.CreatedBy = complaint.CreatedBy;
                dodResultComplaintDo.DateCreated = complaint.DateCreated;
                dodResultComplaintDo.ScoreCard = this.BuildScoreCard(complaint.DodResult);

                dodResultComplaintDos.Add(dodResultComplaintDo);
            }

            return dodResultComplaintDos;
        }

        public async Task<ICollection<DodResultComplaintDO>> GetClosedComplaintsWithResult()
        {
            var dodResultComplaints = await this._dodResultComplaintRepository.GetResolvedWithResult();

            var dodResultComplaintDos = new List<DodResultComplaintDO>();
            foreach (var complaint in dodResultComplaints) {

                var dodResultComplaintDo = new DodResultComplaintDO();
                dodResultComplaintDo.DodResultId = complaint.DodResultId;
                dodResultComplaintDo.DodSubmissionId = complaint.DodResult.SubmissionId;
                dodResultComplaintDo.Id = complaint.Id;
                dodResultComplaintDo.ForScores = complaint.ForScores;
                dodResultComplaintDo.ForComments = complaint.ForComments;
                dodResultComplaintDo.IsResolved = complaint.IsResolved;
                dodResultComplaintDo.IsValidated = complaint.IsValidated;
                dodResultComplaintDo.IsRejected = complaint.IsRejected;
                dodResultComplaintDo.Notes = complaint.Notes;
                dodResultComplaintDo.CreatedBy = complaint.CreatedBy;
                dodResultComplaintDo.DateCreated = complaint.DateCreated;
                dodResultComplaintDo.ScoreCard = this.BuildScoreCard(complaint.DodResult);

                dodResultComplaintDos.Add(dodResultComplaintDo);
            }

            return dodResultComplaintDos;
        }

        public async Task<ICollection<int>> GetJudgedDanceIdsByUser(int userId)
        {
            return await this._dodResultRepository.GetDanceIdsJudgedByUser(userId);
        }

        public async Task<DodTeamCollatedResultPairDO> GetOfficialAndPublicResult()
        {
            var allSubmissions = await this._dodSubmissionRepository.GetAllWithGroupAndResults();

            var dodTeamCollatedResultPair = new DodTeamCollatedResultPairDO();
            dodTeamCollatedResultPair.CollatedOfficialResults = await this._dodResultCollator.CollateOfficialResults(allSubmissions);
            dodTeamCollatedResultPair.CollatedPublicResults = await this._dodResultCollator.CollatePublicResults(allSubmissions);

            return dodTeamCollatedResultPair;

        }

        public async Task<DodResult> GetResultById(int resultId)
        {
            return await this._dodResultRepository.GetByIdWithSubmission(resultId);
        }

        public async Task<DodGroupResultsDO> GetResultsForGroup(int groupId)
        {
            var submmission = await this._dodSubmissionRepository.GetSubmissionWithResultsByGroup(groupId);

            var dodGroupResultsDO = new DodGroupResultsDO();
            dodGroupResultsDO.SubmissionId = submmission.Id;
            dodGroupResultsDO.EmbedLink = submmission.EmbedLink;
            dodGroupResultsDO.EmbedOrigin = submmission.EmbedOrigin;

            dodGroupResultsDO.ScoreCards = new List<DodGroupResultsScoreCardDO>();

            // If there is a complaint against a result then we hide this from the group until it is resolved.
            var visibleResults = submmission.DodResults.Where(r => !r.HasOutstandingComplaint);

            foreach (var result in visibleResults)
            {
                var scoreCard = this.BuildScoreCard(result);

                dodGroupResultsDO.ScoreCards.Add(scoreCard);
            }

            return dodGroupResultsDO;
        }

        public async Task<DodUserResultsDO> GetResultsForJudge(int judgeId)
        {
            var user = await this._dodUserRepository.GetByIdWithResults(judgeId);

            var dodGroupResultsDO = new DodUserResultsDO();
            dodGroupResultsDO.DodUserId = user.Id;
            dodGroupResultsDO.Name = user.Name;
            dodGroupResultsDO.Email = user.Email;

            dodGroupResultsDO.ScoreCards = new List<DodGroupResultsScoreCardDO>();

            foreach (var result in user.DodResults)
            {
                var scoreCard = this.BuildScoreCard(result);

                dodGroupResultsDO.ScoreCards.Add(scoreCard);
            }

            return dodGroupResultsDO;
        }

        public async Task<DodGroupResultsDO> GetResultsForSubmission(int submissionId)
        {
            var submmission = await this._dodSubmissionRepository.GetSubmissionWithResultsById(submissionId);

            var dodGroupResultsDO = new DodGroupResultsDO();
            dodGroupResultsDO.GroupName = submmission.Group.GroupName;
            dodGroupResultsDO.SubmissionId = submmission.Id;
            dodGroupResultsDO.EmbedLink = submmission.EmbedLink;
            dodGroupResultsDO.EmbedOrigin = submmission.EmbedOrigin;

            dodGroupResultsDO.ScoreCards = new List<DodGroupResultsScoreCardDO>();

            foreach (var result in submmission.DodResults)
            {
                var scoreCard = this.BuildScoreCard(result);

                dodGroupResultsDO.ScoreCards.Add(scoreCard);
            }

            return dodGroupResultsDO;
        }

        private DodGroupResultsScoreCardDO BuildScoreCard(DodResult result) 
        {
            var scoreCard = new DodGroupResultsScoreCardDO();
            scoreCard.Comments = result.OverallComments;
            scoreCard.DodResultId = result.Id;
            scoreCard.DodSubmissionId = result.SubmissionId;
            scoreCard.IsOfficial = result.IsOfficial;
            scoreCard.IsIncluded = result.IncludeInScores;
            scoreCard.HasOutstandingComplaint = result.HasOutstandingComplaint;
            scoreCard.ScoreCategories = new List<DodGroupResultsScoreCategoryDO>();

            var music = new DodGroupResultsScoreCategoryDO() { CategoryName = "Music", MaxMarks = 15, Score = result.MusicScore, Comments = result.MusicComments };
            var stepping = new DodGroupResultsScoreCategoryDO() { CategoryName = "Stepping", MaxMarks = 15, Score = result.SteppingScore, Comments = result.SteppingComments };
            var swordhandling = new DodGroupResultsScoreCategoryDO() { CategoryName = "Sword Handling", MaxMarks = 15, Score = result.SwordHandlingScore, Comments = result.SwordHandlingComments };
            var dancetechnique = new DodGroupResultsScoreCategoryDO() { CategoryName = "Dance Technique", MaxMarks = 15, Score = result.DanceTechniqueScore, Comments = result.DanceTechniqueComments };
            var presentation = new DodGroupResultsScoreCategoryDO() { CategoryName = "Presentation", MaxMarks = 15, Score = result.PresentationScore, Comments = result.PresentationComments };
            var buzz = new DodGroupResultsScoreCategoryDO() { CategoryName = "Buzz", MaxMarks = 15, Score = result.BuzzScore, Comments = result.BuzzComments };
            var characters = new DodGroupResultsScoreCategoryDO() { CategoryName = "Characters", MaxMarks = 10, Score = result.CharactersScore, Comments = result.CharactersComments };

            scoreCard.ScoreCategories.Add(music);
            scoreCard.ScoreCategories.Add(stepping);
            scoreCard.ScoreCategories.Add(swordhandling);
            scoreCard.ScoreCategories.Add(dancetechnique);
            scoreCard.ScoreCategories.Add(presentation);
            scoreCard.ScoreCategories.Add(buzz);
            scoreCard.ScoreCategories.Add(characters);

            return scoreCard;
        }
        
    }
}
