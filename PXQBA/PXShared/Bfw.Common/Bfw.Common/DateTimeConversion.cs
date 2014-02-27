using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common
{
    public class DateTimeConversion
    {
        /// <summary>
        /// Since dates capured for module are based on server time, this method 
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="isStartDate">if set to <c>true</c> [is start date].</param>
        /// <returns></returns>
        public static DateTime UtcRelativeAdjustCommon(DateTime dt, string CourseTimeZone)
        {
            try
            {
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(CourseTimeZone);
                TimeZone zone = TimeZone.CurrentTimeZone;
                TimeSpan offset = zone.GetUtcOffset(DateTime.Now);
                // DateTime dt1 = new DateTime();
                TimeSpan time1 = tz.GetUtcOffset(DateTime.Now) - offset;
                dt = dt.Add(time1);
                return dt;
            }
            catch
            {
                return dt;
            }
        }

        /// <summary>
        /// Converts the date from a specific timezone to server timezone
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="CourseTimeZone"></param>
        /// <returns></returns>
        public static DateTime ConvertToServerTime(DateTime dt, string CourseTimeZone)
        {
            try
            {
                TimeZoneInfo courseTZ = TimeZoneInfo.FindSystemTimeZoneById(CourseTimeZone);
                TimeZone serverTZ = TimeZone.CurrentTimeZone;
                TimeSpan offset = serverTZ.GetUtcOffset(DateTime.Now);
                TimeSpan time1 = courseTZ.GetUtcOffset(DateTime.Now) - offset;
                dt = dt.Subtract(time1);
                return dt;
            }
            catch
            {
                return dt;
            }                        
        }

        /// <summary>
        /// Converts the date from a specific timezone to utc
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="courseTimeZone"></param>
        /// <returns></returns>
        public static DateTime ConvertToUtcTime(DateTime dt, string courseTimeZone)
        {
            try
            {
                TimeZoneInfo courseTz = TimeZoneInfo.FindSystemTimeZoneById(courseTimeZone);
                TimeSpan courseTzOffset = courseTz.GetUtcOffset(DateTime.Now);
                dt = dt.Subtract(courseTzOffset);
                return dt;
            }
            catch
            {
                return dt;
            }
        }
    }
}
