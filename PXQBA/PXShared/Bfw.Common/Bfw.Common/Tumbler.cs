using System;
using System.Text;

namespace Bfw.Common
{
    /// <summary>
    /// A tumbler class.
    /// </summary>
    public static class Tumbler
    {
        static private char _beforeStart = '_';
        static private char _start = 'a';
        static private char _middle = 'm';
        static private char _end = 'z';

        /// <summary>
        /// GetTumbler: Will return a new sequence when start and end is not known.
        /// <returns>String</returns>
        /// </summary>
        public static string GetTumbler()
        {
            return GetTumbler(null, null);
        }

        /// <summary>
        /// GetTumbler: Will return a new sequence when start is known but end is not known.
        /// <returns>String</returns>
        /// </summary>
        public static string GetTumbler(string start)
        {
            return GetTumbler(start, null);
        }

        /// <summary>
        /// GetTumbler: Will return a new sequence when start and end is known.
        /// <returns>String</returns>
        /// </summary>
        public static string GetTumbler(string start, string end)
        {
            StringBuilder tumbler = new StringBuilder();
            BuildTumbler(start ?? string.Empty, end ?? string.Empty, 0, tumbler);
            return tumbler.ToString();
        }

        private static void BuildTumbler(string start, string end, int index, StringBuilder tumbler)
        {
            if (index >= start.Length && index >= end.Length)
            {
                // Need to go to the next level
                tumbler.Append(_start);
            }
            else if (index >= start.Length)
            {
                Prev(end, index, tumbler);
            }
            else if (index >= end.Length)
            {
                Next(start, index, tumbler);
            }
            else if (start[index] + 1 < end[index] && start[index] >= _start)
            {
                tumbler.Append((char)(start[index] + 1));
            }
            else
            {
                // Recurse
                tumbler.Append(start[index]);
                BuildTumbler(start, end, index + 1, tumbler);
            }
        }

        private static void Next(string old, int index, StringBuilder tumbler)
        {
            if (old[index] < _end)
            {
                tumbler.Append((char)(old[index] + 1));
            }
            else if (index + 1 >= old.Length)
            {
                tumbler.Append(_end);
                tumbler.Append(_start);
            }
            else
            {
                tumbler.Append(old[index]);
                Next(old, index + 1, tumbler);
            }
        }

        private static void Prev(string old, int index, StringBuilder tumbler)
        {
            if (old[index] > _start)
            {
                tumbler.Append((char)(old[index] - 1));
            }
            else if (index + 1 >= old.Length)
            {
                tumbler.Append(_beforeStart);
                tumbler.Append(_middle);
            }
            else
            {
                tumbler.Append(old[index]);
                Prev(old, index + 1, tumbler);
            }
        }
    }
}