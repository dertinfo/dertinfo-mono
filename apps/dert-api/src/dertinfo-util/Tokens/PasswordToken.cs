using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Util.Tokens
{
    public static class PasswordToken
    {
        public static string Generate() {
            return new Guid().ToString();
        }
    }
}
