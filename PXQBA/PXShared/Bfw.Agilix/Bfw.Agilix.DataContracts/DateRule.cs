using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Helper class to make sure code confroms to Agilix Date/Time contraints
    /// </summary>
    public static class DateRule
    {
        /// <summary>
        /// Minimum date/time for Agilix is 1753-01-01T00:00:00Z
        /// </summary>
        public static readonly DateTime MinDate = DateTime.Parse("1753-01-01T00:00:00Z");

        /// <summary>
        /// Max date/time for Agilix is 9999-12-31T00:00:00Z
        /// </summary>
        public static readonly DateTime MaxDate = DateTime.Parse("9999-12-31T00:00:00Z");

        /// <summary>
        /// Returns true if date is between MinDate and MaxDate
        /// </summary>
        /// <param name="date">date to validate</param>
        /// <returns>true if date is between MinDate and MaxDate</returns>
        public static bool Validate(DateTime date)
        {
            return (date >= MinDate) && (date <= MaxDate);
        }

        /// <summary>
        /// Formats dates to conform with DLAP.
        /// </summary>
        /// <param name="date">formateed datetime string.</param>
        /// <returns></returns>
        public static string Format(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }
}
