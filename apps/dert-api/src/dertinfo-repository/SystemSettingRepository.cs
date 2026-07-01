using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Repository
{

    public interface ISystemSettingRepository : IRepository<SystemSetting, int>
    {

    }

    public class SystemSettingRepository : BaseRepository<SystemSetting, int, DertInfoContext>, ISystemSettingRepository
    {

        public SystemSettingRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {

        }
    }
}
