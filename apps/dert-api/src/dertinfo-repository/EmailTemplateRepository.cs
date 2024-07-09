using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Repository
{
    public interface IEmailTemplateRepository : IRepository<EmailTemplate, int>
    {
    }

    public class EmailTemplateRepository : BaseRepository<EmailTemplate, int, DertInfoContext>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        { }
    }
}