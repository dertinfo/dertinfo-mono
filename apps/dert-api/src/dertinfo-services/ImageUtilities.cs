
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace DertInfo.Services
{
    public interface IImageUtilities
    {
        byte[] ConvertStringToByteArray(string imageString);

    }

    public class ImageUtilities : IImageUtilities
    {
        public byte[] ConvertStringToByteArray(string imageString)
        {
            byte[] bytes = Convert.FromBase64String(imageString);
            return bytes;
        }

    }
}
