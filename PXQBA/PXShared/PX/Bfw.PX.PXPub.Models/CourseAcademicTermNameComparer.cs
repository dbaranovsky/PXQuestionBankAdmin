using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Implements IComparer to compare academic terms by names.
    /// </summary>
    public class CourseAcademicTermNameComparer : IComparer<string>
    {
        /// <summary>
        /// Compare academic terms' names
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(string x, string y)
        {
            if (string.IsNullOrEmpty(x))
                return -1;
            else if (string.IsNullOrEmpty(y))
                return 1;

            var termX = x.Split(' ');
            var termY = y.Split(' ');
            return CompareTerm(termX, termY);
        }

        /// <summary>
        /// Compare academic terms
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareTerm(string[] x, string[] y)
        {
            if (x.Length != 2)
                return -1;
            else if (y.Length != 2)
                return 1;
            var yearComparison = CompareYear(x[1], y[1]);
            if (yearComparison == 0)
                return CompareSemester(x[0], y[0]);
            else
                return yearComparison;
        }

        /// <summary>
        /// Compare academic terms' semesters
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareSemester(string x, string y)
        {
            var semX = SemesterType.Spring;
            var semY = SemesterType.Spring;
            if (!Enum.TryParse<SemesterType>(x, true, out semX))
                return -1;
            if (!Enum.TryParse<SemesterType>(y, true, out semY))
                return 1;
            return semX.CompareTo(semY);

        }

        /// <summary>
        /// Compare academic terms' years
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareYear(string x, string y)
        {
            int yearX = DateTime.MinValue.Year;
            int yearY = DateTime.MinValue.Year;
            if (!int.TryParse(x, out yearX))
                return -1;
            if (!int.TryParse(y, out yearY))
                return 1;
            return yearX.CompareTo(yearY);
        }
    }
}
