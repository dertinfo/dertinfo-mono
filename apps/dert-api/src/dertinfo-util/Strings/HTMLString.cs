using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DertInfo.Util.Strings
{
    public static class HTMLString
    {
        const string HTML_TAG_PATTERN = "<.*?>";

        public static string StripHTML(string inputString)
        {
           return Regex.Replace(inputString, HTML_TAG_PATTERN, string.Empty);
        }

    }
}
