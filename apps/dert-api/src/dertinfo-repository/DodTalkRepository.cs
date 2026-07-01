using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Repository
{
    public interface IDodTalkRepository : IRepository<DodTalk, int>
    {
    }

    public class DodTalkRepository : BaseRepository<DodTalk, int, DertInfoContext>, IDodTalkRepository
    {
        public DodTalkRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }
    }
}
