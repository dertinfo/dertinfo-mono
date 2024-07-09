using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Util.Strings
{
    public static class FilePaths
    {
        /// <summary>
        /// Gets the file name from a path using the format ~\sdfsdf\sdf\sdf\sdf.pdf
        /// NOT COMPLETE
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileNameFromPath(string path)
        {
            int lastSlashIndex = path.LastIndexOf("\\");

            if(lastSlashIndex > 0)
            {
                return path.Substring(lastSlashIndex + 1, path.Length - lastSlashIndex - 1);
            }
            else
            {
                return path;
            }
        }

        public static string GetExtensionFromFilename(string filename)
        {
            int lastDotIndex = filename.LastIndexOf(".");

            if (lastDotIndex > 0)
            {
                return filename.Substring(lastDotIndex + 1, filename.Length - lastDotIndex - 1);
            }
            else
            {
                return "";
            }
        }


    }
}
