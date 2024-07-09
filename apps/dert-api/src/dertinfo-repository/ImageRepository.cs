using DertInfo.CrossCutting.Auth;
using DertInfo.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DertInfo.Repository
{

    public interface IImageRepository : IRepository<Image, int>
    {

    }

    public class ImageRepository : BaseRepository<Image, int, DertInfoContext>, IImageRepository
    {

        public ImageRepository(DertInfoContext context, IDertInfoUser user) : base(context, user)
        {

        }
    }
}
