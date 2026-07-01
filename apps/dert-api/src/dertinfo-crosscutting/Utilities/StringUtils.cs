using System;
using System.Collections.Generic;
using System.Text;

namespace DertInfo.CrossCutting.Utilities
{
    public static class StringUtils
    {
        public static string CreateStandardLengthNumeric(int desiredLength, int numericSeed)
        {
            string strBaseNumber = numericSeed.ToString();

            int difference = desiredLength - strBaseNumber.Length;

            for (int i = 0; i < difference; i++)
            {
                strBaseNumber = "0" + strBaseNumber;
            }

            return strBaseNumber;
        }
    }
}
