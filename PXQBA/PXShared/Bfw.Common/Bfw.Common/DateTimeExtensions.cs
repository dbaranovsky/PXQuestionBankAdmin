using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Bfw.Common
{
    /// <summary>
    /// Some extension functions for DateTimes.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Find the <see cref="DateTime"/> representing the start of the week containing <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="firstDay">The <see cref="DayOfWeek"/> that is considered to be the first day of the week.</param>
        /// <returns>The start of the week.</returns>
        public static DateTime StartOfWeek(this DateTime date, DayOfWeek firstDay)
        {
            int diff = date.DayOfWeek - firstDay;

            if (diff < 0)
            {
                diff += 7;
            }

            return date.AddDays(-diff).Date.StartOfDay();
        }

        /// <summary>
        /// Find the DateTime representing the end of the week containing <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="firstDay">The <see cref="DayOfWeek"/> that is considered to be the first day of the week.</param>
        /// <returns>The end of the week.</returns>
        public static DateTime EndOfWeek(this DateTime date, DayOfWeek firstDay)
        {
            int diff = (firstDay + 6) - date.DayOfWeek;
            
            return date.AddDays(diff).Date.EndOfDay();
        }

        /// <summary>
        /// Find the <see cref="DateTime"/> representing the start of the month containing <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The start of the month.</returns>
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1, date.Hour, date.Minute, date.Second);
        }

        /// <summary>
        /// Find the <see cref="DateTime"/> representing the end of the month containing <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The end of the month.</returns>
        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999);
        }

        /// <summary>
        /// Used to check whether a date falls between two other datres.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="start">The beginning of the date range.</param>
        /// <param name="end">The end of the date range.</param>
        /// <returns><c>true</c> if <paramref name="date"/> falls between <paramref name="start"/> and <paramref name="end"/>, otherwise <c>false</c></returns>
        public static bool InRange(this DateTime date, DateTime start, DateTime end)
        {
            return (date >= start) && (date <= end);
        }

        /// <summary>
        /// Used to find the beginning of a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The beginning of <paramref name="date"/>.</returns>
        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        /// <summary>
        /// Used to find the end of a given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The end of <paramref name="date"/>.</returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }
        /// <summary>
        /// Returns appropriate adjustment rule for a given year
        /// </summary>
        /// <param name="timeZoneInfo"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static TimeZoneInfo.AdjustmentRule GetAdjustment(this TimeZoneInfo timeZoneInfo,
                                                  int year)
        {
            // Iterate adjustment rules for time zone 
            foreach (TimeZoneInfo.AdjustmentRule adjustment in timeZoneInfo.GetAdjustmentRules())
            {
                // Determine if this adjustment rule covers year desired 
                if (adjustment.DateStart.Year <= year && adjustment.DateEnd.Year >= year)
                    return adjustment;
            }
            return null;
        }

        /// <summary>
        /// Returns the DateTime for a timezone transition (to/from daylight savings) for a specific year
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetTransitionInfo(this TimeZoneInfo.TransitionTime transition, int year)
        {
            if (transition.IsFixedDateRule)
            {
                return new DateTime(year, 
                              transition.Month, 
                              transition.Day, 
                              transition.TimeOfDay.Hour,
                              transition.TimeOfDay.Minute,
                              transition.TimeOfDay.Second);
            }
            else
            {
                // For non-fixed date rules, get local calendar
                Calendar cal = CultureInfo.CurrentCulture.Calendar;
                // Get first day of week for transition 
                // For example, the 3rd week starts no earlier than the 15th of the month 
                int startOfWeek = transition.Week * 7 - 6;
                // What day of the week does the month start on? 
                int firstDayOfWeek = (int)cal.GetDayOfWeek(new DateTime(year, transition.Month, 1));
                // Determine how much start date has to be adjusted 
                int transitionDay;
                int changeDayOfWeek = (int)transition.DayOfWeek;

                if (firstDayOfWeek <= changeDayOfWeek)
                    transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek);
                else
                    transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek);

                // Adjust for months with no fifth week 
                if (transitionDay > cal.GetDaysInMonth(year, transition.Month))
                    transitionDay -= 7;

                return new DateTime(year,
                              transition.Month,
                              transitionDay,
                              transition.TimeOfDay.Hour,
                              transition.TimeOfDay.Minute,
                              transition.TimeOfDay.Second);
            }
            
        }   
    }
}
