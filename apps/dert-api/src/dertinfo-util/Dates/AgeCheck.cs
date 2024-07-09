using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Util.Dates
{
    public class AgeCheck
    {
        #region Constructors

        public AgeCheck()
        {
            this.DaysPerYear = decimal.Parse("365");
        }

        #endregion

        #region Properties

        public decimal DaysPerYear { get; set; }

        #endregion

        /// <summary>
        /// Given a date of birth specify that persons age on a specific date
        /// </summary>
        /// <param name="dob"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetAgeOnDate(DateTime dob, DateTime date)
        {
            //Clean the dates
            DateTime myDateOfBirth = new DateTime(dob.Year, dob.Month, dob.Day);
            DateTime myTargetDate = new DateTime(date.Year, date.Month, date.Day);

            TimeSpan timeBetweenDates = new TimeSpan();
            timeBetweenDates = myTargetDate - myDateOfBirth;

            decimal dYears = timeBetweenDates.Days / this.DaysPerYear;
            int age = Convert.ToInt32(Math.Floor(dYears));

            return age;
            
        }

        /// <summary>
        /// Returns the minimum DateTime that a person will be on the age specified on the date specified.
        /// </summary>
        /// <param name="age"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime GetDateBound(int age, DateTime date)
        {
            //Clean the date
            DateTime myDate = new DateTime(date.Year,date.Month,date.Day);

            //Return the date passed less the subtraction
            return myDate.AddYears(age * -1);
        }
    }
}
