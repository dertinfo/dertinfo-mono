using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.Services.Guards
{
    public static class Guard
    {
        public static void IsNotNull(object o)
        {

            if (o == null) { throw new Exception("Guard Exception - Object is Null"); }
        }
    }
}