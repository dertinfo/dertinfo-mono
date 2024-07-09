using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DertInfo.Util.Images
{
    public class ImageUpload
    {
        public ImageUpload()
        { }

        ///// <summary>
        ///// Function to save a file to the file system but if a name clash occours then increment the filename before the extention by a number.
        ///// </summary>
        ///// <param name="teamimage"></param>
        ///// <param name="absFolderPath"></param>
        ///// <returns></returns>
        //public string SaveToFile(HttpPostedFileBase postedFile, string absFolderPath)
        //{
        //    string filenameNoExt = System.IO.Path.GetFileNameWithoutExtension(postedFile.FileName).Replace(" ",string.Empty);
        //    string fileExtension = System.IO.Path.GetExtension(postedFile.FileName);
        //    string absFullPath = System.IO.Path.Combine(absFolderPath, filenameNoExt + fileExtension);


        //    int i = 0;
        //    bool hasBeenAppended = false;
        //    while(File.Exists(absFullPath))
        //    {
        //        i++;
        //        if (hasBeenAppended) { filenameNoExt = filenameNoExt.Substring(0, filenameNoExt.Length - 1); }
        //        filenameNoExt = filenameNoExt + i.ToString();
        //        hasBeenAppended = true;
        //        absFullPath = System.IO.Path.Combine(absFolderPath, filenameNoExt + fileExtension);
        //    }

        //    postedFile.SaveAs(absFullPath);

        //    return filenameNoExt + fileExtension;
        //}

        //public Byte[] ReturnAsByteArray(HttpPostedFileBase postedFile)
        //{
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        postedFile.InputStream.CopyTo(ms);
        //        byte[] array = ms.GetBuffer();
        //        return array;
        //    }
        //}
    }
}
