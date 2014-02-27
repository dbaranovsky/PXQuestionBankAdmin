using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common
{
    [Serializable]
    /// <summary>
    /// Structure to help with time zone conversions
    /// </summary>
    public class DateTimeWithZone
    {
        private DateTime utcDateTime;
        private readonly TimeZoneInfo timeZone;

        public DateTimeWithZone(DateTime dateTime, TimeZoneInfo timeZone, bool isUTCTime)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

            if (!isUTCTime)
            {//if dateTime is in LocalTime
                utcDateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
            }
            else
            {//if dateTime is already in UniversalTime
                utcDateTime = dateTime;
            }
            this.timeZone = timeZone;
        }

        public DateTime UniversalTime { get { return utcDateTime; } }

        public TimeZoneInfo TimeZone { get { return timeZone; } }

        public DateTime LocalTime
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
            }
            set
            {
                value = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

                utcDateTime = TimeZoneInfo.ConvertTimeToUtc(value, timeZone);
            }
        }

        /// <summary>
        /// When the user sends data from the client as JSON, it is already in ServerTime
        /// When we read data from the server, it is in UniversalTime, and get interpeted by .NET into ServerTime
        /// </summary>
        public DateTime ServerTime
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.Local);
            }
        }
    }
}