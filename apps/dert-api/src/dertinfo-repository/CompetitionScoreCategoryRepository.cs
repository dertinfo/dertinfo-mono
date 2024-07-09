using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface ICompetitionScoreCategoryRepository : IRepository<ScoreCategory, int>
    {
    }

    public class CompetitionScoreCategoryRepository : BaseRepository<ScoreCategory, int, DertInfoContext>, ICompetitionScoreCategoryRepository
    {
        public CompetitionScoreCategoryRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }
    }
}
