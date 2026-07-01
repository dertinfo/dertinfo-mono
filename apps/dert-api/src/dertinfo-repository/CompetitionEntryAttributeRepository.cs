using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface ICompetitionEntryAttributeRepository : IRepository<CompetitionEntryAttribute, int>
    {
    }

    public class CompetitionEntryAttributeRepository : BaseRepository<CompetitionEntryAttribute, int, DertInfoContext>, ICompetitionEntryAttributeRepository
    {
        public CompetitionEntryAttributeRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }
    }
}
